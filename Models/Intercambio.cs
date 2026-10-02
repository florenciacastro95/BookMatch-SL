namespace Bookmatch_SL.Models;
using Bookmatch_SL.Models.Enums;
public class Intercambio
{
    public int Id { get; set; }

    public int UsuarioOrigenId { get; set; }

    public int UsuarioDestinoId { get; set; }

    public int LibroOfrecidoId { get; set; }

    public int LibroSolicitadoId { get; set; }

    public EstadoIntercambio Estado { get; set; }

    public bool Activo { get; set; }

    public DateTime CreatedAt { get; set; }


    // Relaciones con Usuario

    public Usuario UsuarioOrigen { get; set; } = null!;

    public Usuario UsuarioDestino { get; set; } = null!;


    // Relaciones con Libro

    public Libro LibroOfrecido { get; set; } = null!;

    public Libro LibroSolicitado { get; set; } = null!;


    // Relaciones con Mensaje

    public ICollection<Mensaje> Mensajes { get; set; }
        = new List<Mensaje>();
}