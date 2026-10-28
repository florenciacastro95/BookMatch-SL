using Bookmatch_SL.Models;
using Bookmatch_SL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bookmatch_SL.Controllers;

// [Authorize] porque un visitante sin cuenta no puede mandar mensajes (lo dice la narrativa)
[Authorize]
public class MensajeController : Controller
{
    private readonly IRepoMensajes repositorioMensajes;
    private readonly IRepoIntercambio repositorioIntercambio;

    public MensajeController(IRepoMensajes repositorioMensajes, IRepoIntercambio repositorioIntercambio)
    {
        this.repositorioMensajes = repositorioMensajes;
        this.repositorioIntercambio = repositorioIntercambio;
    }


    // recibe el formulario del chat que está en el detalle del intercambio.
    // No tiene vista propia: guarda el mensaje y vuelve al detalle.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Enviar(int intercambioId, string contenido)
    {
        string? identificadorUsuario = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(identificadorUsuario, out int idUsuarioAutenticado))
        {
            return Challenge();
        }

        var intercambio = repositorioIntercambio.ObtenerPorId(intercambioId);
        if (intercambio == null)
        {
            return NotFound("No se encontró el intercambio");
        }

        // el emisor sale de la cookie de sesión y no del formulario,
        // así nadie puede mandar un mensaje haciéndose pasar por otro usuario
        bool esUsuarioOrigen = intercambio.UsuarioOrigenId == idUsuarioAutenticado;
        bool esUsuarioDestino = intercambio.UsuarioDestinoId == idUsuarioAutenticado;

        if (!esUsuarioOrigen && !esUsuarioDestino)
        {
            return Forbid();
        }

        // validación del lado del servidor: aunque el input tenga "required"
        // en el navegador, eso se puede saltear
        if (string.IsNullOrWhiteSpace(contenido))
        {
            TempData["Error"] = "El mensaje no puede estar vacío";
            return RedirectToAction("Detalle", "Intercambio", new { id = intercambioId });
        }

        if (contenido.Length > 500)
        {
            TempData["Error"] = "El mensaje no puede superar los 500 caracteres";
            return RedirectToAction("Detalle", "Intercambio", new { id = intercambioId });
        }

        if (!repositorioMensajes.PermiteMensajes(intercambio))
        {
            TempData["Error"] = "Este intercambio ya terminó, no se pueden enviar más mensajes";
            return RedirectToAction("Detalle", "Intercambio", new { id = intercambioId });
        }

        var nuevoMensaje = new Mensaje();
        nuevoMensaje.IntercambioId = intercambioId;
        nuevoMensaje.UsuarioEmisorId = idUsuarioAutenticado;
        nuevoMensaje.Contenido = contenido;

        int idMensajeCreado = repositorioMensajes.Alta(nuevoMensaje);
        if (idMensajeCreado == 0)
        {
            TempData["Error"] = "No se pudo enviar el mensaje";
        }

        // redirijo en vez de devolver una vista, así si el usuario
        // aprieta F5 no se manda el mismo mensaje dos veces
        return RedirectToAction("Detalle", "Intercambio", new { id = intercambioId });
    }
}
