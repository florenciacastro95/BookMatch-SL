using Bookmatch_SL.Models;

namespace Bookmatch_SL.Repositories.Interfaces;

public interface IRepoMensajes : IRepositorio<Mensaje>
{
    IList<Mensaje> ObtenerPorIntercambio(int intercambioId);
}