using Bookmatch_SL.Models;

namespace Bookmatch_SL.Repositories.Interfaces;

public interface IRepoLibro : IRepositorio<Libro>
{
    IList<Libro> BuscarPorTitulo(string titulo);

    IList<Libro> BuscarPorAutor(string autor);

    IList<Libro> ObtenerPorCategoria(int categoriaId);

    IList<Libro> ObtenerPorUsuario(int usuarioId);

    IList<Libro> ObtenerDisponibles();
    IList<Libro> ObtenerCatalogoPublico(int paginaNro, int tamPagina);

    int ObtenerCantidadCatalogoPublico();

    IList<Libro> ObtenerPorUsuarioPaginado(
        int usuarioId, int paginaNro, int tamPagina);

    int ObtenerCantidadPorUsuario(int usuarioId);

    bool TieneIntercambioBloqueante(int libroId);
}