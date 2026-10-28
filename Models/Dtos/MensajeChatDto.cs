namespace Bookmatch_SL.Models.Dtos;

// lo que el servidor le reenvía a los dos usuarios del intercambio
// cuando se guarda un mensaje nuevo. Solo lleva lo que el chat necesita mostrar,
// no la entidad Mensaje entera (que trae el usuario con su contraseña hasheada, etc.)
public class MensajeChatDto
{
    public int Id { get; set; }

    public int IntercambioId { get; set; }

    public int UsuarioEmisorId { get; set; }

    public string NombreEmisor { get; set; } = string.Empty;

    public string Contenido { get; set; } = string.Empty;

    // ya va formateada para que el JavaScript no tenga que convertir fechas
    public string FechaHoraTexto { get; set; } = string.Empty;
}
