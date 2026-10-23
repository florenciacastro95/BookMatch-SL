using Bookmatch_SL.Models;
using Bookmatch_SL.Models.Enums;
using Bookmatch_SL.Repositories.Interfaces;
using BookmatchSL.Data;
using Microsoft.EntityFrameworkCore;

namespace Bookmatch_SL.Repositories;

public class RepositorioUsuario : IRepoUsuarios
{
    private readonly BookDbContext contexto_bd;

    public RepositorioUsuario(BookDbContext contexto_bd)
    {
        this.contexto_bd = contexto_bd;
    }

    private static string NormalizarEmail(string email)
    {
        return email.Trim().ToLowerInvariant();
    }

    public int Alta(Usuario entidad)
    {
        if (entidad == null)
            return 0;

        entidad.Email = NormalizarEmail(entidad.Email);

        if (EmailExiste(entidad.Email))
            return 0;

        if (string.IsNullOrWhiteSpace(entidad.PasswordHash))
            return 0;

        entidad.Nombre = entidad.Nombre.Trim();

        //wl alta pública siempre crea usuarios comunes. Nunca se permite definir el rol desde el registro.
        entidad.Rol = Rol.Usuario;
        entidad.Activo = true;
        entidad.CreatedAt = DateTime.UtcNow;

        contexto_bd.Usuarios.Add(entidad);
        contexto_bd.SaveChanges();

        return entidad.Id;
    }

    public int Baja(int id)
    {
        var usuario = contexto_bd.Usuarios
            .FirstOrDefault(u => u.Id == id);

        if (usuario == null || !usuario.Activo)
            return 0;

        //baja lógica: conservamos el registro y su historial.
        usuario.Activo = false;

        return contexto_bd.SaveChanges();
    }

    public int Modificacion(Usuario entidad)
    {
        if (entidad == null)
            return 0;

        var usuarioExistente = contexto_bd.Usuarios
            .FirstOrDefault(u => u.Id == entidad.Id);

        if (usuarioExistente == null || !usuarioExistente.Activo)
            return 0;

        string email = NormalizarEmail(entidad.Email);

        if (EmailExiste(email, entidad.Id))
            return 0;

        usuarioExistente.Nombre = entidad.Nombre.Trim();
        usuarioExistente.Email = email;
        usuarioExistente.AvatarUrl = entidad.AvatarUrl;

        //no se modifican aquí ni el hash de la contraseña,ni el rol, ni el estado Activo.

        return contexto_bd.SaveChanges();
    }

    public int ModificacionAdministrativa(Usuario entidad)
    {
        if (entidad == null)
            return 0;

        var usuarioExistente = contexto_bd.Usuarios
            .FirstOrDefault(u => u.Id == entidad.Id);

        if (usuarioExistente == null)
            return 0;

        string email = NormalizarEmail(entidad.Email);

        if (EmailExiste(email, entidad.Id))
            return 0;

        usuarioExistente.Nombre = entidad.Nombre.Trim();
        usuarioExistente.Email = email;
        usuarioExistente.Rol = entidad.Rol;
        usuarioExistente.Activo = entidad.Activo;

        //la administración tampoco puede cambiar hashes mediante el formulario general de edición.

        return contexto_bd.SaveChanges();
    }

    public int ActualizarPasswordHash(int id, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            return 0;

        var usuario = contexto_bd.Usuarios
            .FirstOrDefault(u => u.Id == id && u.Activo);

        if (usuario == null)
            return 0;

        usuario.PasswordHash = passwordHash;

        return contexto_bd.SaveChanges();
    }

    public IList<Usuario> ObtenerLista(
        int paginaNro = 1,
        int tamPagina = 10)
    {
        if (paginaNro < 1)
            paginaNro = 1;

        if (tamPagina < 1)
            tamPagina = 10;

        int omitir = (paginaNro - 1) * tamPagina;

        // Para la administración deben aparecer también los usuarios desactivados.
        return contexto_bd.Usuarios
            .AsNoTracking()
            .OrderBy(u => u.Id)
            .Skip(omitir)
            .Take(tamPagina)
            .ToList();
    }

    public int ObtenerCantidad()
    {
        return contexto_bd.Usuarios.Count();
    }

    public Usuario? ObtenerPorId(int id)
    {
        // No usamos AsNoTracking porque algunos métodos del repositorio necesitan modificar esta entidad
        return contexto_bd.Usuarios
            .FirstOrDefault(u => u.Id == id);
    }

    public Usuario? ObtenerPorEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return null;

        email = NormalizarEmail(email);

        // Incluye inactivos el login comprueba Activo.
        return contexto_bd.Usuarios
            .FirstOrDefault(u => u.Email == email);
    }

    public bool EmailExiste(string email, int? excluirId = null)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        email = NormalizarEmail(email);

        return contexto_bd.Usuarios.Any(u =>
            u.Email == email &&
            (!excluirId.HasValue || u.Id != excluirId.Value));
    }
}