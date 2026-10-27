using Microsoft.AspNetCore.Mvc;
using Bookmatch_SL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Bookmatch_SL.Controllers;

[Authorize]
public class IntercambioController : Controller
{
    private readonly IRepoIntercambio repositorioIntercambio;
    private readonly IRepoLibro repositorioLibro;

    public IntercambioController(IRepoIntercambio repositorioIntercambio, IRepoLibro repositorioLibro)
    {
        this.repositorioIntercambio = repositorioIntercambio;
        this.repositorioLibro = repositorioLibro;
    }




    public IActionResult Index(int pagina = 1)
    {
        string? identificadorUsuario = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(identificadorUsuario, out int idUsuarioAutenticado))
        {
            return Challenge();
        }

        int cantidad = repositorioIntercambio
            .ObtenerCantidadPorUsuario(idUsuarioAutenticado);

        int totalPaginas = (int)Math.Ceiling(cantidad / 10.0);
        if (totalPaginas < 1)
        {
            totalPaginas = 1;
        }
        if (pagina < 1)
        {
            pagina = 1;
        }
        if (pagina > totalPaginas)
        {
            pagina = totalPaginas;
        }

        var intercambiosEncontrados = repositorioIntercambio
            .ObtenerPorUsuario(idUsuarioAutenticado, pagina, 10);
        ViewBag.PaginaActual = pagina;
        ViewBag.TotalPaginas = totalPaginas;

        return View(intercambiosEncontrados);
    }

    [HttpGet]
    public IActionResult Detalle(int id, int pagina = 1)
    {
        string? identificadorUsuario = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(identificadorUsuario, out int idUsuarioAutenticado))
        {
            return Challenge();
        }


        var intercambioEncontrado = repositorioIntercambio.ObtenerPorId(id);
        if (intercambioEncontrado == null)
        {
            return NotFound("No se encontro el intercambio");
        }
        bool esUsuarioOrigen =
      intercambioEncontrado.UsuarioOrigenId == idUsuarioAutenticado;

        bool esUsuarioDestino =
            intercambioEncontrado.UsuarioDestinoId == idUsuarioAutenticado;

        if (!esUsuarioOrigen && !esUsuarioDestino)
        {
            return Forbid();
        }


        if (pagina < 1)
        {
            pagina = 1;
        }

        ViewBag.PaginaActual = pagina;
        return View(intercambioEncontrado);
    }


    [HttpGet]
    public IActionResult Solicitar(int id)
    {
        string? identificadorUsuario = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(identificadorUsuario, out int idUsuarioAutenticado))
        {
            return Challenge();
        }

        var libroSolicitado = repositorioLibro.ObtenerPorId(id);
        if (libroSolicitado == null)
        {
            return NotFound("El libro que buscás no existe o fue eliminado del catálogo");
        }
        if (!libroSolicitado.Activo || !libroSolicitado.Disponible)
        {
            return BadRequest("El libro no está disponible para intercambio.");
        }

        if (libroSolicitado.UsuarioId == idUsuarioAutenticado)
        {
            return BadRequest("No podés solicitar tu propio libro");
        }

        var librosPropios = repositorioLibro.ObtenerPorUsuario(idUsuarioAutenticado);
        var librosParaOfrecer = librosPropios
            .Where(libro => libro.Activo && libro.Disponible)
            .ToList();

        ViewBag.LibroSolicitadoId = libroSolicitado.Id;
        ViewBag.LibroSolicitadoTitulo = libroSolicitado.Titulo;
        ViewBag.LibrosParaOfrecer = librosParaOfrecer;

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Solicitar(int libroSolicitadoId, int libroOfrecidoId)
    {
        string? identificadorUsuario = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(identificadorUsuario, out int idUsuarioAutenticado))
        {
            return Challenge();
        }

        var libroSolicitado = repositorioLibro.ObtenerPorId(libroSolicitadoId);
        var libroOfrecido = repositorioLibro.ObtenerPorId(libroOfrecidoId);

        if (libroSolicitado == null)
        {
            return NotFound("No se encontró el libro solicitado");
        }

        if (libroOfrecido == null)
        {
            ModelState.AddModelError(
                "",
                "Seleccioná un libro propio válido para ofrecer"
            );
        }
        else if (!libroSolicitado.Activo || !libroOfrecido.Activo)
        {
            ModelState.AddModelError(
                "",
                "Lo sentimos. Uno de los libros seleccionados ya no está activo en el catálogo"
            );
        }
        else if (libroOfrecido.UsuarioId != idUsuarioAutenticado ||
                 libroSolicitado.UsuarioId == idUsuarioAutenticado ||
                 libroOfrecido.Id == libroSolicitado.Id)
        {
            ModelState.AddModelError(
                "",
                "Debés ofrecer un libro propio y solicitar uno de otro usuario"
            );
        }
        else if (repositorioIntercambio.ExistePropuestaPendiente(
            idUsuarioAutenticado,
            libroSolicitado.UsuarioId,
            libroOfrecidoId,
            libroSolicitadoId))
        {
            ModelState.AddModelError(
                "",
                "Ya enviaste una propuesta con estos mismos libros. Aguardá la respuesta del otro usuario"
            );
        }
        else if (!libroSolicitado.Disponible || !libroOfrecido.Disponible)
        {
            ModelState.AddModelError(
                "",
                "Lo sentimos. Uno de los libros seleccionados ya no está disponible para intercambio"
            );
        }

        if (ModelState.IsValid)
        {
            var nuevaSolicitud = new Bookmatch_SL.Models.Intercambio();

            nuevaSolicitud.UsuarioOrigenId = idUsuarioAutenticado;
            nuevaSolicitud.UsuarioDestinoId = libroSolicitado.UsuarioId;
            nuevaSolicitud.LibroOfrecidoId = libroOfrecidoId;
            nuevaSolicitud.LibroSolicitadoId = libroSolicitadoId;

            int idIntercambioCreado = repositorioIntercambio.Alta(nuevaSolicitud);
            if (idIntercambioCreado > 0)
            {
                return RedirectToAction("Detalle", new { id = idIntercambioCreado });
            }

            ModelState.AddModelError(
                "",
                "No se pudo enviar la propuesta. Los datos pudieron cambiar; verificá la disponibilidad de los libros y las propuestas pendientes"
            );
        }

        var librosPropios = repositorioLibro.ObtenerPorUsuario(idUsuarioAutenticado);

        ViewBag.LibrosParaOfrecer = librosPropios
            .Where(libro => libro.Activo && libro.Disponible)
            .ToList();

        ViewBag.LibroSolicitadoId = libroSolicitado.Id;
        ViewBag.LibroSolicitadoTitulo = libroSolicitado.Titulo;

        return View();
    }




    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Cancelar(int id)
    {
        string? identificadorUsuario = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(identificadorUsuario, out int idUsuarioAutenticado))
        {
            return Challenge();
        }

        int resultado = repositorioIntercambio.Cancelar(id, idUsuarioAutenticado);
        if (resultado == 0)
        {
            TempData["Error"] = "No se pudo cancelar el intercambio";
        }
        return RedirectToAction("Detalle", new { id });
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Aceptar(int id)
    {
        string? identificadorUsuario = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(identificadorUsuario, out int idUsuarioAutenticado))
        {

            return Challenge();
        }

        int resultado = repositorioIntercambio.Aceptar(id, idUsuarioAutenticado);
        if (resultado > 0)
        {
            TempData["Exito"] = "Aceptaste el intercambio. Ya podés coordinar la entrega.";
        }
        else
        {
            TempData["Error"] = "No se pudo aceptar el intercambio. Comprobá que la propuesta siga activa y que ambos libros sigan disponibles";
        }

        return RedirectToAction("Detalle", new { id });
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Rechazar(int id)
    {
        string? identificadorUsuario = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(identificadorUsuario, out int idUsuarioAutenticado))
        {
            return Challenge();
        }

        int resultado = repositorioIntercambio.Rechazar(id, idUsuarioAutenticado);
        if (resultado > 0)
        {
            TempData["Exito"] = "Rechazaste la propuesta de intercambio";
            return RedirectToAction("Index");
        }
        else
        {
            TempData["Error"] = "No se pudo rechazar la propuesta. Verificá que la solicitud siga pendiente";
        }

        return RedirectToAction("Detalle", new { id });
    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult MarcarExitoso(int id)
    {
        string? identificadorUsuario = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(identificadorUsuario, out int idUsuarioAutenticado))
        {
            return Challenge();
        }

        int resultado = repositorioIntercambio.MarcarExitoso(id, idUsuarioAutenticado);
        if (resultado > 0)
        {
            TempData["Exito"] = "El intercambio se registró como exitoso";
        }
        else
        {
            TempData["Error"] = "No se pudo finalizar el intercambio. Comprobá que siga activo";
        }

        return RedirectToAction("Detalle", new { id });
    }
}
