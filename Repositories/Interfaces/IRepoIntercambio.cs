using Bookmatch_SL.Models;
using Bookmatch_SL.Models.Enums;

namespace Bookmatch_SL.Repositories.Interfaces;

public interface IRepoIntercambio : IRepositorio<Intercambio>
{
    IList<Intercambio> ObtenerPorUsuario(int usuarioId);

    IList<Intercambio> ObtenerPorEstado(EstadoIntercambio estado);
    int Rechazar(int idIntercambio, int idUsuarioAutenticado);
    int Aceptar(int idIntercambio, int idUsuarioAutenticado);
    int Cancelar(int idIntercambio, int idUsuarioAutenticado);
    int MarcarExitoso(int idIntercambio, int idUsuarioAutenticado);
    IList<Intercambio> ObtenerEnviados(int usuarioId);
    IList<Intercambio> ObtenerRecibidos(int usuarioId);

}