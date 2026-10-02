using Bookmatch_SL.Models;

namespace Bookmatch_SL.Repositories.Interfaces;

public interface IRepoLibro : IRepositorio<Libro>
{
    IList<Libro> BuscarPorTitulo(string titulo);

    IList<Libro> BuscarPorAutor(string autor);

    IList<Libro> ObtenerPorCategoria(int categoriaId);

    IList<Libro> ObtenerPorUsuario(int usuarioId);

    IList<Libro> ObtenerDisponibles();
}