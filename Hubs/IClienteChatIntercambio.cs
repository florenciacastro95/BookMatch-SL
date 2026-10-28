using Bookmatch_SL.Models.Dtos;

namespace Bookmatch_SL.Hubs;

// métodos que el servidor puede ejecutar en el JavaScript del navegador.
// Al usar una interfaz (hub tipado) no escribimos los nombres como texto suelto:
// si me equivoco en un nombre, no compila, en vez de fallar en silencio.
public interface IClienteChatIntercambio
{
    Task RecibirMensaje(MensajeChatDto mensajeChat);

    Task RecibirError(string descripcionError);
}
