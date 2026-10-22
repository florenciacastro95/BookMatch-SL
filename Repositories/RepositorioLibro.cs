using Bookmatch_SL.Models;
using Bookmatch_SL.Repositories.Interfaces;
using BookmatchSL.Data;
using Microsoft.EntityFrameworkCore;

namespace Bookmatch_SL.Repositories;

public class RepositorioLibro : IRepoLibro
{
    private readonly BookDbContext contexto_bd;

    public RepositorioLibro(BookDbContext contexto_bd)
    {
        this.contexto_bd = contexto_bd;
    }

    public int Alta(Libro entidad)
    {
        if (entidad == null) return 0;

        entidad.Activo = true;
        entidad.CreatedAt = DateTime.Now;
        entidad.Disponible = true;

        contexto_bd.Libros.Add(entidad);
        contexto_bd.SaveChanges();
        return entidad.Id;
    }

    public int Baja(int id)
    {
        var libro = ObtenerPorId(id);
        if (libro == null) return 0;

        libro.Activo = false;
        return contexto_bd.SaveChanges();
    }

    public int Modificacion(Libro entidad)
    {
        var libroExistente = contexto_bd.Libros.Find(entidad.Id);
        if (libroExistente == null || !libroExistente.Activo) return 0;

        libroExistente.Titulo = entidad.Titulo;
        libroExistente.Autor = entidad.Autor;
        libroExistente.Editorial = entidad.Editorial;
        libroExistente.Idioma = entidad.Idioma;
        libroExistente.Formato = entidad.Formato;
        libroExistente.Aclaracion = entidad.Aclaracion;
        libroExistente.PortadaUrl = entidad.PortadaUrl;
        libroExistente.EstadoConservacion = entidad.EstadoConservacion;
        libroExistente.CategoriaId = entidad.CategoriaId;
        libroExistente.Disponible = entidad.Disponible;

        return contexto_bd.SaveChanges();
    }

    public IList<Libro> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
    {
        if (paginaNro < 1) paginaNro = 1;
        if (tamPagina < 1) tamPagina = 10;

        int omitir = (paginaNro - 1) * tamPagina;

        return contexto_bd.Libros
            .Include(l => l.Categoria)
            .Include(l => l.Usuario)
            .Where(l => l.Activo)
            .OrderByDescending(l => l.Id)
            .Skip(omitir)
            .Take(tamPagina)
            .ToList();
    }

    public int ObtenerCantidad()
    {
        return contexto_bd.Libros.Count(l => l.Activo);
    }

    public Libro? ObtenerPorId(int id)
    {
        return contexto_bd.Libros
            .Include(l => l.Categoria)
            .Include(l => l.Usuario)
            .FirstOrDefault(l => l.Id == id && l.Activo);
    }

    public IList<Libro> BuscarPorTitulo(string titulo)
    {
        return contexto_bd.Libros
            .Include(l => l.Categoria)
            .Include(l => l.Usuario)
            .Where(l => l.Activo && l.Titulo.Contains(titulo))
            .ToList();
    }

    public IList<Libro> BuscarPorAutor(string autor)
    {
        return contexto_bd.Libros
            .Include(l => l.Categoria)
            .Include(l => l.Usuario)
            .Where(l => l.Activo && l.Autor.Contains(autor))
            .ToList();
    }

    public IList<Libro> ObtenerPorCategoria(int categoriaId)
    {
        return contexto_bd.Libros
            .Include(l => l.Categoria)
            .Include(l => l.Usuario)
            .Where(l => l.Activo && l.CategoriaId == categoriaId)
            .ToList();
    }

    public IList<Libro> ObtenerPorUsuario(int usuarioId)
    {
        return contexto_bd.Libros
            .Include(l => l.Categoria)
            .Where(l => l.Activo && l.UsuarioId == usuarioId)
            .ToList();
    }

    public IList<Libro> ObtenerDisponibles()
    {
        return contexto_bd.Libros
            .Include(l => l.Categoria)
            .Include(l => l.Usuario)
            .Where(l => l.Activo && l.Disponible)
            .ToList();
    }
}