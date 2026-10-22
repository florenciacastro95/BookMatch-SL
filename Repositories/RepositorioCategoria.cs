using Bookmatch_SL.Models;
using Bookmatch_SL.Repositories.Interfaces;
using BookmatchSL.Data;
using Microsoft.EntityFrameworkCore;

namespace Bookmatch_SL.Repositories;

public class RepositorioCategoria : IRepoCategoria
{
    private readonly BookDbContext contexto_bd;

    public RepositorioCategoria(BookDbContext contexto_bd)
    {
        this.contexto_bd = contexto_bd;
    }

    public int Alta(Categoria entidad)
    {
        if (entidad == null) return 0;

        entidad.Activo = true;

        contexto_bd.Categorias.Add(entidad);
        contexto_bd.SaveChanges();
        return entidad.Id;
    }

    public int Baja(int id)
    {
        var categoria = ObtenerPorId(id);
        if (categoria == null) return 0;

        categoria.Activo = false;
        return contexto_bd.SaveChanges();
    }

    public int Modificacion(Categoria entidad)
    {
        var categoriaExistente = contexto_bd.Categorias.Find(entidad.Id);
        if (categoriaExistente == null || !categoriaExistente.Activo) return 0;

        categoriaExistente.Nombre = entidad.Nombre;

        return contexto_bd.SaveChanges();
    }

    public IList<Categoria> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
    {
        if (paginaNro < 1) paginaNro = 1;
        if (tamPagina < 1) tamPagina = 10;

        int omitir = (paginaNro - 1) * tamPagina;

        return contexto_bd.Categorias
            .Where(c => c.Activo)
            .OrderBy(c => c.Nombre)
            .Skip(omitir)
            .Take(tamPagina)
            .ToList();
    }

    public int ObtenerCantidad()
    {
        return contexto_bd.Categorias.Count(c => c.Activo);
    }

    public Categoria? ObtenerPorId(int id)
    {
        return contexto_bd.Categorias
            .FirstOrDefault(c => c.Id == id && c.Activo);
    }
}