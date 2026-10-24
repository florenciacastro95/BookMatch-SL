using Microsoft.AspNetCore.Mvc;
using Bookmatch_SL.Repositories.Interfaces;

namespace Bookmatch_SL.Controllers;
/*
public class IntercambioController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}


 ------------------ controller recibe este repo y devuelve los 10 primeros intercambiso. 
*/

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
        if (pagina < 1)
        {
            pagina = 1;
        }

        int cantidad = repositorioIntercambio.ObtenerCantidad();
        int totalPaginas = (int)Math.Ceiling(cantidad / 10.0);

        if (totalPaginas < 1)
        {
            totalPaginas = 1;
        }
        if (pagina > totalPaginas)
        {
            pagina = totalPaginas;
        }
        IList<Bookmatch_SL.Models.Intercambio> intercambiosEncontrados =
            repositorioIntercambio.ObtenerLista(pagina, 10);
        ViewBag.PaginaActual = pagina;
        ViewBag.TotalPaginas = totalPaginas;
        return View(intercambiosEncontrados);
    }

    [HttpGet]
    public IActionResult Detalle(int id, int pagina = 1)
    {
        var intercambioEncontrado = repositorioIntercambio.ObtenerPorId(id);
        if (intercambioEncontrado == null)
        {
            return NotFound("Ups, no hay intercambios aún");
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
        var libroSolicitado = repositorioLibro.ObtenerPorId(id);

        if (libroSolicitado == null)
        {
            return NotFound("Lo sentimos, no se encontró el libro solicitado.");
        }

        if (!libroSolicitado.Disponible)
        {
            return BadRequest("Lo sentimos, el libro ya no está disponible para intercambio.");
        }

        ViewBag.LibroSolicitadoId = libroSolicitado.Id;
        ViewBag.LibroSolicitadoTitulo = libroSolicitado.Titulo;
        return View();
    }

    public IActionResult ProbarConexion()
    {
        var intercambiosEncontrados = repositorioIntercambio.ObtenerLista(1, 10);

        return Json(intercambiosEncontrados);
    }
}