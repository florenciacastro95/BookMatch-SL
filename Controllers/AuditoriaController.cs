using Bookmatch_SL.Models.Enums;
using BookmatchSL.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bookmatch_SL.Controllers;

[Authorize(Roles = nameof(Rol.Administrador))]
public class AuditoriaController : Controller
{
    private readonly BookDbContext contexto;

    public AuditoriaController(BookDbContext contexto)
    {
        this.contexto = contexto;
    }

    [HttpGet]
    public IActionResult Index(int pagina = 1)
    {
        const int tamPagina = 20;

        if (pagina < 1)
            pagina = 1;

        int totalRegistros = contexto.Auditorias.Count();

        var registros = contexto.Auditorias
            .AsNoTracking()
            .OrderByDescending(a => a.FechaHora)
            .ThenByDescending(a => a.Id)
            .Skip((pagina - 1) * tamPagina)
            .Take(tamPagina)
            .ToList();

        ViewBag.PaginaActual = pagina;
        ViewBag.TotalPaginas = Math.Max(
            1,
            (int)Math.Ceiling((double)totalRegistros / tamPagina));

        return View(registros);
    }
}