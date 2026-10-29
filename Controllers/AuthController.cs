using System.Security.Claims;
using Bookmatch_SL.Models;
using Bookmatch_SL.Models.Enums;
using Bookmatch_SL.Repositories.Interfaces;
using Bookmatch_SL.ViewModels;
using BookmatchSL.Data;
using Bookmatch_SL.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bookmatch_SL.Controllers;

public class AuthController : Controller
{
    private readonly IRepoUsuarios repoUsuarios;
    private readonly IPasswordHasher<Usuario> passwordHasher;
    private readonly ServicioAvatar servicioAvatar;
    private readonly BookDbContext contexto;

    public AuthController(
        IRepoUsuarios repoUsuarios,
        IPasswordHasher<Usuario> passwordHasher,
        ServicioAvatar servicioAvatar,
        BookDbContext contexto)
    {
        this.repoUsuarios = repoUsuarios;
        this.passwordHasher = passwordHasher;
        this.servicioAvatar = servicioAvatar;
        this.contexto = contexto;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Registro()
    {
        return View();
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Registro(RegistroViewModel modelo)
    {
        if (!ModelState.IsValid)
            return View(modelo);

        if (repoUsuarios.EmailExiste(modelo.Email))
        {
            ModelState.AddModelError(
                nameof(modelo.Email),
                "Ya existe una cuenta con ese email.");

            return View(modelo);
        }

        string? avatarUrl = null;

        try
        {
            avatarUrl = await servicioAvatar.GuardarAsync(modelo.Avatar);
        }
        catch (InvalidDataException ex)
        {
            ModelState.AddModelError(nameof(modelo.Avatar), ex.Message);
            return View(modelo);
        }

        var usuario = new Usuario
        {
            Nombre = modelo.Nombre.Trim(),
            Email = modelo.Email.Trim().ToLowerInvariant(),
            Rol = Rol.Usuario,
            Activo = true,
            CreatedAt = DateTime.UtcNow,
            AvatarUrl = avatarUrl
        };

        usuario.PasswordHash = passwordHasher.HashPassword(
            usuario,
            modelo.Password);

        try
        {
            int idCreado = repoUsuarios.Alta(usuario);

            if (idCreado == 0)
            {
                servicioAvatar.EliminarSiExiste(avatarUrl);

                ModelState.AddModelError(
                    string.Empty,
                    "No se pudo registrar la cuenta. Revisá el email.");

                return View(modelo);
            }
        }
        catch (DbUpdateException)
        {
            servicioAvatar.EliminarSiExiste(avatarUrl);

            ModelState.AddModelError(
                string.Empty,
                "No se pudo registrar la cuenta. Puede que el email ya esté en uso.");

            return View(modelo);
        }

        TempData["Mensaje"] =
            "Tu cuenta se creó correctamente. Ya podés iniciar sesión.";

        return RedirectToAction(nameof(Login));
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        return View(new LoginViewModel
        {
            ReturnUrl = returnUrl
        });
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel modelo)
    {
        if (!ModelState.IsValid)
            return View(modelo);

        var usuario = repoUsuarios.ObtenerPorEmail(modelo.Email);

        if (usuario == null ||
    !usuario.Activo ||
    string.IsNullOrWhiteSpace(usuario.PasswordHash))
        {
            RegistrarEventoAcceso(
                usuario?.Id,
                "INICIO_SESION_RECHAZADO");

            ModelState.AddModelError(
                string.Empty,
                "El email o la contraseña son incorrectos.");

            return View(modelo);
        }

        var resultado = passwordHasher.VerifyHashedPassword(
            usuario,
            usuario.PasswordHash,
            modelo.Password);

        if (resultado == PasswordVerificationResult.Failed)
        {
            RegistrarEventoAcceso(usuario.Id, "INICIO_SESION_RECHAZADO");

            ModelState.AddModelError(
                string.Empty,
                "El email o la contraseña son incorrectos.");

            return View(modelo);
        }

        if (resultado == PasswordVerificationResult.SuccessRehashNeeded)
        {
            usuario.PasswordHash = passwordHasher.HashPassword(
                usuario,
                modelo.Password);

            repoUsuarios.ActualizarPasswordHash(
                usuario.Id,
                usuario.PasswordHash);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.Nombre),
            new(ClaimTypes.Email, usuario.Email),
            new(ClaimTypes.Role, usuario.Rol.ToString())
        };

        var identidad = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identidad);

        var propiedades = new AuthenticationProperties
        {
            IsPersistent = modelo.Recordarme,
            ExpiresUtc = DateTimeOffset.UtcNow.Add(
                modelo.Recordarme
                    ? TimeSpan.FromDays(30)
                    : TimeSpan.FromHours(8))
        };

        RegistrarEventoAcceso(usuario.Id, "INICIO_SESION");

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            propiedades);

        if (!string.IsNullOrWhiteSpace(modelo.ReturnUrl) &&
            Url.IsLocalUrl(modelo.ReturnUrl))
        {
            return LocalRedirect(modelo.ReturnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        int? idUsuario = ObtenerIdUsuarioAutenticado();

        if (idUsuario.HasValue)
            RegistrarEventoAcceso(idUsuario.Value, "CIERRE_SESION");

        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    [HttpGet]
    public IActionResult Perfil()
    {
        int? id = ObtenerIdUsuarioAutenticado();

        if (!id.HasValue)
            return RedirectToAction(nameof(Login));

        var usuario = repoUsuarios.ObtenerPorId(id.Value);

        if (usuario == null || !usuario.Activo)
            return RedirectToAction(nameof(Login));

        var modelo = new PerfilViewModel
        {
            Nombre = usuario.Nombre,
            Email = usuario.Email,
            AvatarActualUrl = usuario.AvatarUrl
        };

        return View(modelo);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Perfil(PerfilViewModel modelo)
    {
        int? id = ObtenerIdUsuarioAutenticado();

        if (!id.HasValue)
            return RedirectToAction(nameof(Login));

        var usuario = repoUsuarios.ObtenerPorId(id.Value);

        if (usuario == null || !usuario.Activo)
            return RedirectToAction(nameof(Login));

        modelo.AvatarActualUrl = usuario.AvatarUrl;

        if (!ModelState.IsValid)
            return View(modelo);

        if (repoUsuarios.EmailExiste(modelo.Email, usuario.Id))
        {
            ModelState.AddModelError(
                nameof(modelo.Email),
                "Ese email ya está en uso.");

            return View(modelo);
        }

        string? avatarNuevo = null;

        try
        {
            avatarNuevo = await servicioAvatar.GuardarAsync(modelo.Avatar);
        }
        catch (InvalidDataException ex)
        {
            ModelState.AddModelError(nameof(modelo.Avatar), ex.Message);
            return View(modelo);
        }

        string? avatarAnterior = usuario.AvatarUrl;

        var datosModificados = new Usuario
        {
            Id = usuario.Id,
            Nombre = modelo.Nombre.Trim(),
            Email = modelo.Email.Trim().ToLowerInvariant(),
            AvatarUrl = avatarNuevo ?? avatarAnterior
        };

        try
        {
            repoUsuarios.Modificacion(datosModificados);
        }
        catch (DbUpdateException)
        {
            servicioAvatar.EliminarSiExiste(avatarNuevo);

            ModelState.AddModelError(
                string.Empty,
                "No se pudieron guardar los cambios. Revisá el email.");

            return View(modelo);
        }

        if (avatarNuevo != null)
            servicioAvatar.EliminarSiExiste(avatarAnterior);

        TempData["Mensaje"] = "El perfil se actualizó correctamente.";

        return RedirectToAction(nameof(Perfil));
    }

    [Authorize]
    [HttpGet]
    public IActionResult CambiarPassword()
    {
        return View();
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CambiarPassword(CambiarPasswordViewModel modelo)
    {
        if (!ModelState.IsValid)
            return View(modelo);

        int? id = ObtenerIdUsuarioAutenticado();

        if (!id.HasValue)
            return RedirectToAction(nameof(Login));

        var usuario = repoUsuarios.ObtenerPorId(id.Value);

        if (usuario == null || !usuario.Activo)
            return RedirectToAction(nameof(Login));

        var resultado = passwordHasher.VerifyHashedPassword(
            usuario,
            usuario.PasswordHash,
            modelo.PasswordActual);

        if (resultado == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError(
                nameof(modelo.PasswordActual),
                "La contraseña actual es incorrecta.");

            return View(modelo);
        }

        string nuevoHash = passwordHasher.HashPassword(
            usuario,
            modelo.PasswordNueva);

        int resultadoActualizacion = repoUsuarios.ActualizarPasswordHash(
            usuario.Id,
            nuevoHash);

       if (resultadoActualizacion == 0)
        {
            ModelState.AddModelError(
                string.Empty,
                "No se pudo actualizar la contraseña.");

            return View(modelo);
        }

        TempData["Mensaje"] = "La contraseña se cambió correctamente.";

        return RedirectToAction(nameof(Perfil));
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Denegado()
    {
        return View();
    }

    private int? ObtenerIdUsuarioAutenticado()
    {
        string? valor = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return int.TryParse(valor, out int id)
            ? id
            : null;
    }

    private void RegistrarEventoAcceso(int? idUsuario, string accion)
    {
        contexto.Auditorias.Add(new Auditoria
        {
            IdUsuario = idUsuario,
            Accion = accion,
            Entidad = "Usuario",
            IdEntidad = idUsuario,
            FechaHora = DateTime.UtcNow
        });

        contexto.SaveChanges();
    }
}