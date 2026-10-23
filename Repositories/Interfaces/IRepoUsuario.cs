using Bookmatch_SL.Models;

namespace Bookmatch_SL.Repositories.Interfaces;

public interface IRepoUsuarios : IRepositorio<Usuario>
{
    Usuario? ObtenerPorEmail(string email);

    bool EmailExiste(string email, int? excluirId = null);

    int ModificacionAdministrativa(Usuario entidad);

    int ActualizarPasswordHash(int id, string passwordHash);
}