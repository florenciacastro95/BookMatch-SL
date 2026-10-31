using Bookmatch_SL.Models;
using Bookmatch_SL.Models.Enums;
using Bookmatch_SL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookmatch_SL.Controllers;

// las categorías las administra solo el admin; un usuario común solo las elige al cargar un libro
[Authorize(Roles = nameof(Rol.Administrador))]
public class CategoriaController : Controller
{
    private const int tamPagina = 10;

    private readonly IRepoCategoria repositorioCategoria;

    public CategoriaController(IRepoCategoria repositorioCategoria)
    {
        this.repositorioCategoria = repositorioCategoria;
    }

    // solo devuelve la página; el listado, el alta, la edición y la baja los hace Vue por AJAX
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    // manda una sola página de categorías en JSON, así nunca se traen todas juntas
    [HttpGet]
    public IActionResult Listar(int pagina = 1)
    {
        int cantidad = repositorioCategoria.ObtenerCantidad();
        int totalPaginas = Math.Max(1, (int)Math.Ceiling(cantidad / (double)tamPagina));

        pagina = Math.Clamp(pagina, 1, totalPaginas);

        var categoriasEncontradas = repositorioCategoria.ObtenerLista(pagina, tamPagina)
            .Select(categoria => new
            {
                id = categoria.Id,
                nombre = categoria.Nombre
            });

        return Ok(new
        {
            categorias = categoriasEncontradas,
            paginaActual = pagina,
            totalPaginas = totalPaginas
        });
    }

    // si el id es 0 es un alta, si no es una modificación (igual que GuardarAjax del ejemplo de clase)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult GuardarAjax(int id, string? nombre)
    {
        string nombreIngresado = (nombre ?? string.Empty).Trim();

        // se valida acá aunque Vue ya haya revisado, porque el navegador se puede saltear
        if (nombreIngresado == string.Empty)
        {
            ModelState.AddModelError("Nombre", "Pone un nombre para la categoria");
        }
        else if (nombreIngresado.Length > 255)
        {
            ModelState.AddModelError("Nombre", "El nombre es muy largo");
        }
        else if (repositorioCategoria.ExisteNombre(nombreIngresado, id))
        {
            ModelState.AddModelError("Nombre", "Ya hay una categoria con ese nombre");
        }

        // con los errores en el ModelState, Vue sabe qué mensaje mostrar debajo de cada campo
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (id == 0)
        {
            var categoriaNueva = new Categoria { Nombre = nombreIngresado };
            int idCategoriaCreada = repositorioCategoria.Alta(categoriaNueva);

            if (idCategoriaCreada == 0)
            {
                ModelState.AddModelError("Nombre", "No se pudo guardar, proba de nuevo");
                return BadRequest(ModelState);
            }

            return Ok(new { id = idCategoriaCreada, nombre = categoriaNueva.Nombre });
        }

        var categoriaEncontrada = repositorioCategoria.ObtenerPorId(id);

        if (categoriaEncontrada == null)
        {
            return NotFound();
        }

        categoriaEncontrada.Nombre = nombreIngresado;
        repositorioCategoria.Modificacion(categoriaEncontrada);

        return Ok(new { id = categoriaEncontrada.Id, nombre = categoriaEncontrada.Nombre });
    }

    // baja lógica: la categoría queda inactiva y deja de aparecer en el listado
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DesactivarAjax(int id)
    {
        var categoriaEncontrada = repositorioCategoria.ObtenerPorId(id);

        if (categoriaEncontrada == null)
        {
            return NotFound();
        }

        // si quedan libros con esta categoría no se puede dar de baja, se quedarían sin categoría
        if (repositorioCategoria.TieneLibrosActivos(id))
        {
            ModelState.AddModelError("Categoria", "No se puede desactivar porque tiene libros cargados");
            return BadRequest(ModelState);
        }

        if (repositorioCategoria.Baja(id) == 0)
        {
            ModelState.AddModelError("Categoria", "No se pudo desactivar, proba de nuevo");
            return BadRequest(ModelState);
        }

        return Ok();
    }
}
