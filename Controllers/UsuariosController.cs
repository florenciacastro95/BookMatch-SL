using Bookmatch_SL.Models;
using Bookmatch_SL.Models.Enums;
using Bookmatch_SL.Repositories.Interfaces;
using Bookmatch_SL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bookmatch_SL.Controllers;

[Authorize(Roles = nameof(Rol.Administrador))]
public class UsuariosController : Controller
{
    private readonly IRepoUsuarios repoUsuarios;

    public UsuariosController(IRepoUsuarios repoUsuarios)
    {
        this.repoUsuarios = repoUsuarios;
    }

    [HttpGet]
    public IActionResult Index(int pagina = 1)
    {
        const int tamPagina = 10;

        if (pagina < 1)
            pagina = 1;

        var usuarios = repoUsuarios.ObtenerLista(pagina, tamPagina);
        int totalRegistros = repoUsuarios.ObtenerCantidad();

        var modelo = new UsuarioAdminIndexViewModel
        {
            PaginaActual = pagina,
            TotalPaginas = Math.Max(
                1,
                (int)Math.Ceiling((double)totalRegistros / tamPagina)),

            Usuarios = usuarios.Select(u => new UsuarioAdminItemViewModel
            {
                Id = u.Id,
                Nombre = u.Nombre,
                Email = u.Email,
                Rol = u.Rol.ToString(),
                Activo = u.Activo,
                CreatedAt = u.CreatedAt,
                AvatarUrl = u.AvatarUrl
            }).ToList()
        };

        return View(modelo);
    }

    [HttpGet]
    public IActionResult Editar(int id)
    {
        var usuario = repoUsuarios.ObtenerPorId(id);

        if (usuario == null)
            return NotFound();

        var modelo = new UsuarioAdminEditarViewModel
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Email = usuario.Email,
            Rol = usuario.Rol,
            Activo = usuario.Activo
        };

        return View(modelo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Editar(
        int id,
        UsuarioAdminEditarViewModel modelo)
    {
        if (id != modelo.Id)
            return BadRequest();

        var usuarioActual = repoUsuarios.ObtenerPorId(id);

        if (usuarioActual == null)
            return NotFound();

        if (!Enum.IsDefined(modelo.Rol))
        {
            ModelState.AddModelError(
                nameof(modelo.Rol),
                "El rol seleccionado no es válido.");
        }

        // No permitimos que el administrador se quite su propio rol
        // ni que desactive su cuenta desde este formulario.
        string? idAutenticado =
            User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (idAutenticado == id.ToString() &&
            (modelo.Rol != Rol.Administrador || !modelo.Activo))
        {
            ModelState.AddModelError(
                string.Empty,
                "No podés quitarte el rol de administrador ni desactivar tu cuenta.");
        }

        if (repoUsuarios.EmailExiste(modelo.Email, modelo.Id))
        {
            ModelState.AddModelError(
                nameof(modelo.Email),
                "Ese email ya está en uso.");
        }

        if (!ModelState.IsValid)
            return View(modelo);

        var usuarioModificado = new Usuario
        {
            Id = modelo.Id,
            Nombre = modelo.Nombre.Trim(),
            Email = modelo.Email.Trim().ToLowerInvariant(),
            Rol = modelo.Rol,
            Activo = modelo.Activo
        };

        try
        {
            repoUsuarios.ModificacionAdministrativa(usuarioModificado);
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(
                string.Empty,
                "No se pudieron guardar los cambios. Revisá el email.");

            return View(modelo);
        }

        TempData["Mensaje"] = "Los datos del usuario se actualizaron.";

        return RedirectToAction(nameof(Index));
    }
}