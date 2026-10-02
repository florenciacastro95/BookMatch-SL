namespace Bookmatch_SL.Models;
using Bookmatch_SL.Models.Enums;
public class Usuario
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public Rol Rol { get; set; }

    public string? AvatarUrl { get; set; }

    public bool Activo { get; set; }

    public DateTime CreatedAt { get; set; }



    public ICollection<Libro> Libros { get; set; } = new List<Libro>();

    public ICollection<Intercambio> IntercambiosOrigen { get; set; }
        = new List<Intercambio>();

    public ICollection<Intercambio> IntercambiosDestino { get; set; }
        = new List<Intercambio>();

    public ICollection<Mensaje> MensajesEnviados { get; set; }
        = new List<Mensaje>();
}