
using Bookmatch_SL.Models;
using Bookmatch_SL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bookmatch_SL.Controllers;

[Authorize]
public class LibroController : Controller
{
private readonly IRepoLibro repoLibro;
private readonly IRepoCategoria repoCategoria;

public LibroController(IRepoLibro repoLibro, IRepoCategoria repoCategoria)
{
    this.repoLibro = repoLibro;
    this.repoCategoria = repoCategoria;
}

    [AllowAnonymous]
    public IActionResult Index(int pagina = 1)
    {
        int tamPagina = 10;
        var libros = repoLibro.ObtenerLista(pagina, tamPagina);
        ViewBag.TotalRegistros = repoLibro.ObtenerCantidad();
        ViewBag.PaginaActual = pagina;
        ViewBag.TotalPaginas = (int)Math.Ceiling((double)ViewBag.TotalRegistros / tamPagina);
        return View(libros);
    }

    [AllowAnonymous]
    public IActionResult Details(int id)
    {
        var libro = repoLibro.ObtenerPorId(id);
        if (libro == null) return NotFound();
        return View(libro);
    }

    public IActionResult Create()
    {
        ViewBag.Categorias = repoCategoria.ObtenerLista(1, 100);
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Libro libro)
    {
        // Obtener el ID del usuario logueado desde los claims
        var claimId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (claimId == null) return RedirectToAction("Login", "Auth");

        libro.UsuarioId = int.Parse(claimId);

        if (ModelState.IsValid)
        {
            repoLibro.Alta(libro);
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Categorias = repoCategoria.ObtenerLista(1, 100);
        return View(libro);
    }

    public IActionResult Edit(int id)
    {
        var libro = repoLibro.ObtenerPorId(id);
        if (libro == null) return NotFound();

        var claimId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (libro.UsuarioId.ToString() != claimId && !User.IsInRole("Administrador"))
        {
            return Forbid();
        }

        ViewBag.Categorias = repoCategoria.ObtenerLista(1, 100);
        return View(libro);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Libro libro)
    {
        if (id != libro.Id) return NotFound();

        var libroOriginal = repoLibro.ObtenerPorId(id);
        if (libroOriginal == null) return NotFound();

        var claimId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (libroOriginal.UsuarioId.ToString() != claimId && !User.IsInRole("Administrador"))
        {
            return Forbid();
        }

        // Mantener datos que no vienen del formulario
        libro.UsuarioId = libroOriginal.UsuarioId;

        if (ModelState.IsValid)
        {
            repoLibro.Modificacion(libro);
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Categorias = repoCategoria.ObtenerLista(1, 100);
        return View(libro);
    }

    public IActionResult Delete(int id)
    {
        var libro = repoLibro.ObtenerPorId(id);
        if (libro == null) return NotFound();

        var claimId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (libro.UsuarioId.ToString() != claimId && !User.IsInRole("Administrador"))
        {
            return Forbid();
        }

        return View(libro);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var libro = repoLibro.ObtenerPorId(id);
        if (libro != null)
        {
            var claimId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (libro.UsuarioId.ToString() == claimId || User.IsInRole("Administrador"))
            {
                repoLibro.Baja(id);
            }
        }
        return RedirectToAction(nameof(Index));
    }
}