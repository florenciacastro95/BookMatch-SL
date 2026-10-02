using Bookmatch_SL.Models;
using Bookmatch_SL.Models.Enums;

namespace Bookmatch_SL.Repositories.Interfaces;

public interface IRepoIntercambio : IRepositorio<Intercambio>
{
    IList<Intercambio> ObtenerPorUsuario(int usuarioId);

    IList<Intercambio> ObtenerPorEstado(EstadoIntercambio estado);
}