
using System.Security.Claims;
using Bookmatch_SL.Models;
using Bookmatch_SL.Repositories.Interfaces;
using Bookmatch_SL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bookmatch_SL.Controllers;

[Authorize]
public class LibroController : Controller
{
    private readonly IRepoLibro repoLibro;
    private readonly IRepoCategoria repoCategoria;
    private readonly ServicioPortada servicioPortada;

    public LibroController(
        IRepoLibro repoLibro,
        IRepoCategoria repoCategoria,
        ServicioPortada servicioPortada)
    {
        this.repoLibro = repoLibro;
        this.repoCategoria = repoCategoria;
        this.servicioPortada = servicioPortada;
    }

    private void CargarCategorias()
    {
        ViewBag.Categorias = repoCategoria.ObtenerLista(1, 100);
    }

    private bool EsPropietarioOAdministrador(Libro libro)
    {
        string? idUsuario = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        return libro.UsuarioId.ToString() == idUsuario
               || User.IsInRole("Administrador");
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Index(int pagina = 1)
    {
        const int tamPagina = 10;

        int totalRegistros =
            repoLibro.ObtenerCantidadCatalogoPublico();

        int totalPaginas = Math.Max(
            1,
            (int)Math.Ceiling((double)totalRegistros / tamPagina));

        pagina = Math.Clamp(pagina, 1, totalPaginas);

        var libros = repoLibro.ObtenerCatalogoPublico(
            pagina, tamPagina);

        ViewBag.TotalRegistros = totalRegistros;
        ViewBag.PaginaActual = pagina;
        ViewBag.TotalPaginas = totalPaginas;

        return View(libros);
    }


    [AllowAnonymous]
    [HttpGet]
    public IActionResult Details(int id)
    {
        var libro = repoLibro.ObtenerPorId(id);

        if (libro == null)
            return NotFound();

        return View(libro);
    }

    [Authorize]
    [HttpGet]
    public IActionResult MiBiblioteca(int pagina = 1)
    {
        if (!int.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out int usuarioId))
        {
            return RedirectToAction("Login", "Auth");
        }

        const int tamPagina = 10;

        int totalRegistros =
            repoLibro.ObtenerCantidadPorUsuario(usuarioId);

        int totalPaginas = Math.Max(
            1,
            (int)Math.Ceiling((double)totalRegistros / tamPagina));

        pagina = Math.Clamp(pagina, 1, totalPaginas);

        var libros = repoLibro.ObtenerPorUsuarioPaginado(
            usuarioId, pagina, tamPagina);

        ViewBag.TotalRegistros = totalRegistros;
        ViewBag.PaginaActual = pagina;
        ViewBag.TotalPaginas = totalPaginas;

        return View(libros);
    }

    [HttpGet]
    public IActionResult Create()
    {
        CargarCategorias();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        Libro libro, IFormFile? portadaArchivo)
    {
        if (!int.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out int usuarioId))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (portadaArchivo == null || portadaArchivo.Length == 0)
        {
            ModelState.AddModelError(
                string.Empty,
                "Tenés que cargar una imagen de portada.");
        }

        if (repoCategoria.ObtenerPorId(libro.CategoriaId) == null)
        {
            ModelState.AddModelError(
                nameof(libro.CategoriaId),
                "Seleccioná una categoría válida.");
        }

        if (!ModelState.IsValid)
        {
            CargarCategorias();
            return View(libro);
        }

        string? portadaGuardada = null;

        try
        {
            portadaGuardada =
                await servicioPortada.GuardarAsync(portadaArchivo!);

            libro.PortadaUrl = portadaGuardada;
            libro.UsuarioId = usuarioId;

            repoLibro.Alta(libro);

            return RedirectToAction(nameof(MiBiblioteca));
        }
        catch (InvalidDataException ex)
        {
            servicioPortada.EliminarSiExiste(portadaGuardada);

            ModelState.AddModelError(string.Empty, ex.Message);

            CargarCategorias();
            return View(libro);
        }
        catch (DbUpdateException)
        {
            servicioPortada.EliminarSiExiste(portadaGuardada);

            ModelState.AddModelError(
                string.Empty,
                "No se pudo guardar el libro. Revisá los datos ingresados.");

            CargarCategorias();
            return View(libro);
        }
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var libro = repoLibro.ObtenerPorId(id);

        if (libro == null)
            return NotFound();

        if (!EsPropietarioOAdministrador(libro))
            return Forbid();

        CargarCategorias();

        ViewBag.DisponibilidadBloqueada =
            repoLibro.TieneIntercambioBloqueante(id);

        return View(libro);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id, Libro libro, IFormFile? portadaArchivo)
    {
        if (id != libro.Id)
            return BadRequest();

        var libroOriginal = repoLibro.ObtenerPorId(id);

        if (libroOriginal == null)
            return NotFound();

        if (!EsPropietarioOAdministrador(libroOriginal))
            return Forbid();

        bool disponibilidadBloqueada =
            repoLibro.TieneIntercambioBloqueante(id);

        // La disponibilidad se obtiene de la base de datos
        // si existe un intercambio que la bloquea.
        if (disponibilidadBloqueada)
        {
            libro.Disponible = libroOriginal.Disponible;
        }

        if (repoCategoria.ObtenerPorId(libro.CategoriaId) == null)
        {
            ModelState.AddModelError(
                nameof(libro.CategoriaId),
                "Seleccioná una categoría válida.");
        }

        if (!ModelState.IsValid)
        {
            CargarCategorias();
            ViewBag.DisponibilidadBloqueada =
                disponibilidadBloqueada;

            return View(libro);
        }

        string? portadaNueva = null;

        try
        {
            if (portadaArchivo != null && portadaArchivo.Length > 0)
            {
                portadaNueva =
                    await servicioPortada.GuardarAsync(portadaArchivo);

                libro.PortadaUrl = portadaNueva;
            }
            else
            {
                // Si no se sube otra imagen, se conserva la actual.
                libro.PortadaUrl = libroOriginal.PortadaUrl;
            }

            // El propietario nunca se toma del formulario.
            libro.UsuarioId = libroOriginal.UsuarioId;

            repoLibro.Modificacion(libro);

            if (portadaNueva != null)
            {
                servicioPortada.EliminarSiExiste(
                    libroOriginal.PortadaUrl);
            }

            return RedirectToAction(nameof(MiBiblioteca));
        }
        catch (InvalidDataException ex)
        {
            servicioPortada.EliminarSiExiste(portadaNueva);

            ModelState.AddModelError(string.Empty, ex.Message);

            CargarCategorias();
            ViewBag.DisponibilidadBloqueada =
                disponibilidadBloqueada;

            return View(libro);
        }
        catch (DbUpdateException)
        {
            servicioPortada.EliminarSiExiste(portadaNueva);

            ModelState.AddModelError(
                string.Empty,
                "No se pudieron guardar los cambios del libro.");

            CargarCategorias();
            ViewBag.DisponibilidadBloqueada =
                disponibilidadBloqueada;

            return View(libro);
        }
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        var libro = repoLibro.ObtenerPorId(id);

        if (libro == null)
            return NotFound();

        if (!EsPropietarioOAdministrador(libro))
            return Forbid();

        return View(libro);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var libro = repoLibro.ObtenerPorId(id);

        if (libro == null)
            return NotFound();

        if (!EsPropietarioOAdministrador(libro))
            return Forbid();

        if (repoLibro.TieneIntercambioBloqueante(id))
        {
            TempData["ErrorLibro"] =
                "No podés dar de baja este libro porque participa de un intercambio.";

            return RedirectToAction(nameof(MiBiblioteca));
        }
        repoLibro.Baja(id);

        return RedirectToAction(nameof(MiBiblioteca));
    }
}
