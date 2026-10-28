namespace Bookmatch_SL.Models.Dtos;

// lo que manda el navegador cuando alguien escribe en el chat.
// No trae el emisor a propósito: el emisor lo saca el servidor de la cookie,
// así nadie puede mandar un mensaje haciéndose pasar por otro.
public class MensajeEnviadoDto
{
    public int IntercambioId { get; set; }

    public string Contenido { get; set; } = string.Empty;
}
