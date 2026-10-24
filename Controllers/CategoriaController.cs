using Bookmatch_SL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Bookmatch_SL.Controllers;

public class CategoriaController : Controller
{
    private readonly IRepoCategoria repositorioCategoria;

    public CategoriaController(IRepoCategoria repositorioCategoria)
    {
        this.repositorioCategoria = repositorioCategoria;
    }

    public IActionResult Index(int pagina = 1)
    {
        if (pagina < 1)
        {
            pagina = 1;
        }

        int cantidad = repositorioCategoria.ObtenerCantidad();
        int totalPaginas = (int)Math.Ceiling(cantidad / 10.0);

        if (totalPaginas < 1)
        {
            totalPaginas = 1;
        }

        if (pagina > totalPaginas)
        {
            pagina = totalPaginas;
        }

        var categoriasEncontradas = repositorioCategoria.ObtenerLista(pagina, 10);

        ViewBag.PaginaActual = pagina;
        ViewBag.TotalPaginas = totalPaginas;

        return View(categoriasEncontradas);
    }


    [HttpGet]
    public IActionResult Crear()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Crear(string? nombre)
    {
        var categoriaNueva = new Bookmatch_SL.Models.Categoria();

        if (string.IsNullOrWhiteSpace(nombre))
        {
            ModelState.AddModelError("Nombre", "Ingresá el nombre de la categoría");
            return View(categoriaNueva);
        }

        categoriaNueva.Nombre = nombre.Trim();

        if (categoriaNueva.Nombre.Length > 255)
        {
            ModelState.AddModelError("Nombre", "El nombre ingresado supera los 255 caracteres");
            return View(categoriaNueva);
        }

        if (repositorioCategoria.ExisteNombre(categoriaNueva.Nombre))
        {
            ModelState.AddModelError("Nombre", "Ya existe una categoría con ese nombre. Reintentá con otro nombre");
            return View(categoriaNueva);
        }

        int idCategoriaCreada = repositorioCategoria.Alta(categoriaNueva);

        if (idCategoriaCreada == 0)
        {
            ModelState.AddModelError("Nombre", "No se pudo guardar la categoría. Reintente nuevamente");
            return View(categoriaNueva);
        }



        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Editar(int id)
    {
        var categoriaEncontrada = repositorioCategoria.ObtenerPorId(id);

        if (categoriaEncontrada == null)
        {
            return NotFound("No se encontró la categoría.");
        }

        return View(categoriaEncontrada);
    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Editar(int id, string? nombre)
    {
        var categoriaEncontrada = repositorioCategoria.ObtenerPorId(id);

        if (categoriaEncontrada == null)
        {
            return NotFound("No se encontró la categoría");
        }

        if (string.IsNullOrWhiteSpace(nombre))
        {
            ModelState.AddModelError("Nombre", "Ingresá el nombre de la categoría");
            return View(categoriaEncontrada);
        }

        categoriaEncontrada.Nombre = nombre.Trim();

        if (categoriaEncontrada.Nombre.Length > 255)
        {
            ModelState.AddModelError("Nombre", "El nombre no puede superar los 255 caracteres");
            return View(categoriaEncontrada);
        }

        if (repositorioCategoria.ExisteNombre(categoriaEncontrada.Nombre, id))
        {
            ModelState.AddModelError("Nombre", "Ya existe una categoría con ese nombre");
            return View(categoriaEncontrada);
        }



        repositorioCategoria.Modificacion(categoriaEncontrada);

        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Desactivar(int id)
    {
        var categoriaEncontrada = repositorioCategoria.ObtenerPorId(id);

        if (categoriaEncontrada == null)
        {
            return NotFound("No se encontró la categoría");
        }

        return View(categoriaEncontrada);
    }

    [HttpPost]
    [ActionName("Desactivar")]
    [ValidateAntiForgeryToken]
    public IActionResult ConfirmarDesactivacion(int id)
    {
        var categoriaEncontrada = repositorioCategoria.ObtenerPorId(id);

        if (categoriaEncontrada == null)
        {
            return NotFound("No se encontró la categoría");
        }

        if (repositorioCategoria.TieneLibrosActivos(id))
        {
            bool tieneLibrosActivos = repositorioCategoria.TieneLibrosActivos(id);

            if (tieneLibrosActivos)
            {
                string mensajeError = "La categoría tiene libros activos, no es posible desactivarla";
                ModelState.AddModelError("", mensajeError);
                return View("Desactivar", categoriaEncontrada);

            }
            ;

            return View("Desactivar", categoriaEncontrada);
        }

        int resultado = repositorioCategoria.Baja(id);

        if (resultado == 0)
        {
            ModelState.AddModelError("", "No se pudo desactivar la categoría");
            return View("Desactivar", categoriaEncontrada);
        }

        return RedirectToAction("Index");
    }


}


