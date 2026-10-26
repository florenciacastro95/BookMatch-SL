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

        var intercambiosDelUsuario =
            repositorioIntercambio.ObtenerPorUsuario(idUsuarioAutenticado);

        int cantidad = intercambiosDelUsuario.Count;
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

        int cantidadOmitida = (pagina - 1) * 10;


        var intercambiosEncontrados = intercambiosDelUsuario
            .Skip(cantidadOmitida)
            .Take(10)
            .ToList();

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
        if (libroSolicitado == null)
        {
            return NotFound("No se encontró el libro solicitado");
        }

        var nuevaSolicitud = new Bookmatch_SL.Models.Intercambio();
        nuevaSolicitud.UsuarioOrigenId = idUsuarioAutenticado;
        nuevaSolicitud.UsuarioDestinoId = libroSolicitado.UsuarioId;
        nuevaSolicitud.LibroOfrecidoId = libroOfrecidoId;
        nuevaSolicitud.LibroSolicitadoId = libroSolicitadoId;

        int idIntercambioCreado = repositorioIntercambio.Alta(nuevaSolicitud);
        if (idIntercambioCreado == 0)
        {
            ModelState.AddModelError(
                "",
                "No se pudo enviar la propuesta. Comprobá que los libros estén disponibles y que ofrezcas uno propio."
            );

            var librosPropios = repositorioLibro.ObtenerPorUsuario(idUsuarioAutenticado);
            ViewBag.LibrosParaOfrecer = librosPropios
                .Where(libro => libro.Activo && libro.Disponible)
                .ToList();
            ViewBag.LibroSolicitadoId = libroSolicitado.Id;
            ViewBag.LibroSolicitadoTitulo = libroSolicitado.Titulo;

            return View();
        }

        return RedirectToAction("Detalle", new { id = idIntercambioCreado });
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
