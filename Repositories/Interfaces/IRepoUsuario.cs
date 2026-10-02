using Bookmatch_SL.Models;

namespace Bookmatch_SL.Repositories.Interfaces;

public interface IRepoUsuarios : IRepositorio<Usuario>
{
    Usuario? ObtenerPorEmail(string email);
}