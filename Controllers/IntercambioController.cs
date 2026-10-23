using Microsoft.AspNetCore.Mvc;



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

using Bookmatch_SL.Repositories.Interfaces;




public class IntercambioController : Controller
{
    private readonly IRepoIntercambio repositorioIntercambio;

    public IntercambioController(IRepoIntercambio repositorioIntercambio)
    {
        this.repositorioIntercambio = repositorioIntercambio;
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
    public IActionResult Detalle(int id)
    {
        var intercambioEncontrado = repositorioIntercambio.ObtenerPorId(id);

        if (intercambioEncontrado == null)
        {
            return NotFound("Ups, no hay intercambios aún");
        }

        return View(intercambioEncontrado);
    }


    [HttpGet]
    public IActionResult Solicitar()
    {
        return View();
    }

    public IActionResult ProbarConexion()
    {
        var intercambiosEncontrados = repositorioIntercambio.ObtenerLista(1, 10);

        return Json(intercambiosEncontrados);
    }
}