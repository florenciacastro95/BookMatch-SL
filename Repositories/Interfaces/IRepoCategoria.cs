using Bookmatch_SL.Models;

namespace Bookmatch_SL.Repositories.Interfaces;

public interface IRepoCategoria : IRepositorio<Categoria>
{
    bool ExisteNombre(string nombre, int idExcluir = 0);
    bool TieneLibrosActivos(int id);
}




