using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookmatch_SL.Controllers.Api;

[ApiController]
[Route("api/perfil")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class PerfilApiController : ControllerBase
{
    [HttpGet("me")]
    public IActionResult MiPerfil()
    {
        return Ok(new
        {
            id = User.FindFirstValue(ClaimTypes.NameIdentifier),
            nombre = User.FindFirstValue(ClaimTypes.Name),
            email = User.FindFirstValue(ClaimTypes.Email),
            rol = User.FindFirstValue(ClaimTypes.Role)
        });
    }

    [HttpGet("administracion")]
    [Authorize(Roles = "Administrador")]
    public IActionResult SoloAdministrador()
    {
        return Ok(new
        {
            mensaje = "Acceso autorizado para administrador."
        });
    }
}