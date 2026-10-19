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

    public IActionResult Index()
    {
        IList<Bookmatch_SL.Models.Intercambio> intercambiosEncontrados =
        repositorioIntercambio.ObtenerLista(1, 10);
        return View(intercambiosEncontrados);
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