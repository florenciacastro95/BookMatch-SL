namespace Bookmatch_SL.Models;

public class Mensaje
{
    public int Id { get; set; }

    public int IntercambioId { get; set; }

    public int UsuarioEmisorId { get; set; }

    public string Contenido { get; set; } = string.Empty;

    public DateTime FechaHora { get; set; }


    // Relaciones

    public Intercambio Intercambio { get; set; } = null!;

    public Usuario UsuarioEmisor { get; set; } = null!;
}