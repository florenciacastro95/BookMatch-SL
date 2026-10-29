using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Bookmatch_SL.Models;
using Bookmatch_SL.Repositories.Interfaces;
using Bookmatch_SL.ViewModels;
using BookmatchSL.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Bookmatch_SL.Controllers.Api;

[ApiController]
[Route("api/auth")]
public class AuthApiController : ControllerBase
{
    private readonly IRepoUsuarios repoUsuarios;
    private readonly IPasswordHasher<Usuario> passwordHasher;
    private readonly BookDbContext contexto;
    private readonly IConfiguration configuracion;

    public AuthApiController(
        IRepoUsuarios repoUsuarios,
        IPasswordHasher<Usuario> passwordHasher,
        BookDbContext contexto,
        IConfiguration configuracion)
    {
        this.repoUsuarios = repoUsuarios;
        this.passwordHasher = passwordHasher;
        this.contexto = contexto;
        this.configuracion = configuracion;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login([FromBody] ApiLoginRequest modelo)
    {
        var usuario = repoUsuarios.ObtenerPorEmail(modelo.Email);

        if (usuario == null ||
    !usuario.Activo ||
    string.IsNullOrWhiteSpace(usuario.PasswordHash))
        {
            RegistrarIntentoApiFallido(usuario?.Id);

            return Unauthorized(new
            {
                mensaje = "El email o la contraseña son incorrectos."
            });
        }

        var resultado = passwordHasher.VerifyHashedPassword(
            usuario,
            usuario.PasswordHash,
            modelo.Password);

        if (resultado == PasswordVerificationResult.Failed)
        {
            RegistrarIntentoApiFallido(usuario.Id);

            return Unauthorized(new
            {
                mensaje = "El email o la contraseña son incorrectos."
            });
        }

        string emisor = configuracion["Jwt:Issuer"]!;
        string audiencia = configuracion["Jwt:Audience"]!;
        string clave = configuracion["Jwt:Key"]!;

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nombre),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.Rol.ToString())
        };

        var claveFirma = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(clave));

        var credenciales = new SigningCredentials(
            claveFirma,
            SecurityAlgorithms.HmacSha256);

        DateTime expira = DateTime.UtcNow.AddMinutes(30);

        var token = new JwtSecurityToken(
            issuer: emisor,
            audience: audiencia,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expira,
            signingCredentials: credenciales);

        string tokenGenerado =
            new JwtSecurityTokenHandler().WriteToken(token);

        contexto.Auditorias.Add(new Auditoria
        {
            IdUsuario = usuario.Id,
            Accion = "INICIO_SESION_API",
            Entidad = "Usuario",
            IdEntidad = usuario.Id,
            FechaHora = DateTime.UtcNow
        });

        contexto.SaveChanges();

        return Ok(new
        {
            accessToken = tokenGenerado,
            tokenType = "Bearer",
            expiresAtUtc = expira
        });
    }

    private void RegistrarIntentoApiFallido(int? idUsuario)
    {
        contexto.Auditorias.Add(new Auditoria
        {
            IdUsuario = idUsuario,
            Accion = "INICIO_SESION_API_RECHAZADO",
            Entidad = "Usuario",
            IdEntidad = idUsuario,
            FechaHora = DateTime.UtcNow
        });

        contexto.SaveChanges();
    }
}