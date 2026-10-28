using Bookmatch_SL.Models;
using Bookmatch_SL.Models.Dtos;
using Bookmatch_SL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Bookmatch_SL.Hubs;

// Hub del chat de cada intercambio. Mantiene abierta la conexión con el navegador,
// así cuando uno escribe al otro le aparece el mensaje en el momento sin recargar.
// [Authorize] usa la cookie de sesión de la web, la misma con la que el usuario ya entró.
[Authorize]
public class ChatIntercambioHub : Hub<IClienteChatIntercambio>
{
    private readonly IRepoMensajes repositorioMensajes;
    private readonly IRepoIntercambio repositorioIntercambio;

    public ChatIntercambioHub(IRepoMensajes repositorioMensajes, IRepoIntercambio repositorioIntercambio)
    {
        this.repositorioMensajes = repositorioMensajes;
        this.repositorioIntercambio = repositorioIntercambio;
    }


    // cada intercambio es una "sala" (un grupo de SignalR).
    // Así el mensaje le llega solo a los dos usuarios de ese intercambio y no a todos.
    public async Task UnirseAlChat(int intercambioId)
    {
        int idUsuarioAutenticado = ObtenerIdUsuarioAutenticado();

        var intercambioEncontrado = repositorioIntercambio.ObtenerPorId(intercambioId);

        if (intercambioEncontrado == null || !EsParticipante(intercambioEncontrado, idUsuarioAutenticado))
        {
            await Clients.Caller.RecibirError("No participás de este intercambio");
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, NombreDelGrupo(intercambioId));
    }


    // lo llama el JavaScript cuando el usuario aprieta "Enviar mensaje".
    // Primero se guarda en la base (el hub no guarda nada, se crea y se destruye
    // en cada llamada) y recién después se reenvía a la sala.
    public async Task EnviarMensaje(MensajeEnviadoDto mensajeEnviado)
    {
        int idUsuarioAutenticado = ObtenerIdUsuarioAutenticado();

        var intercambioEncontrado = repositorioIntercambio.ObtenerPorId(mensajeEnviado.IntercambioId);

        if (intercambioEncontrado == null || !EsParticipante(intercambioEncontrado, idUsuarioAutenticado))
        {
            await Clients.Caller.RecibirError("No participás de este intercambio");
            return;
        }

        // misma validación del servidor que en MensajeController
        if (string.IsNullOrWhiteSpace(mensajeEnviado.Contenido))
        {
            await Clients.Caller.RecibirError("El mensaje no puede estar vacío");
            return;
        }

        if (mensajeEnviado.Contenido.Length > 500)
        {
            await Clients.Caller.RecibirError("El mensaje no puede superar los 500 caracteres");
            return;
        }

        if (!repositorioMensajes.PermiteMensajes(intercambioEncontrado))
        {
            await Clients.Caller.RecibirError("Este intercambio ya terminó, no se pueden enviar más mensajes");
            return;
        }

        var nuevoMensaje = new Mensaje();
        nuevoMensaje.IntercambioId = mensajeEnviado.IntercambioId;
        nuevoMensaje.UsuarioEmisorId = idUsuarioAutenticado;
        nuevoMensaje.Contenido = mensajeEnviado.Contenido;

        int idMensajeCreado = repositorioMensajes.Alta(nuevoMensaje);
        if (idMensajeCreado == 0)
        {
            await Clients.Caller.RecibirError("No se pudo enviar el mensaje");
            return;
        }

        // lo vuelvo a buscar para tener el nombre del emisor sacado de la base,
        // no un nombre que haya escrito el usuario
        var mensajeGuardado = repositorioMensajes.ObtenerPorId(idMensajeCreado);
        if (mensajeGuardado == null)
        {
            return;
        }

        var mensajeParaElChat = new MensajeChatDto();
        mensajeParaElChat.Id = mensajeGuardado.Id;
        mensajeParaElChat.IntercambioId = mensajeGuardado.IntercambioId;
        mensajeParaElChat.UsuarioEmisorId = mensajeGuardado.UsuarioEmisorId;
        mensajeParaElChat.NombreEmisor = mensajeGuardado.UsuarioEmisor.Nombre;
        mensajeParaElChat.Contenido = mensajeGuardado.Contenido;
        mensajeParaElChat.FechaHoraTexto = mensajeGuardado.FechaHora.ToString("dd/MM/yyyy HH:mm");

        // se lo mando a toda la sala, incluido el que escribió,
        // así a los dos les aparece igual
        await Clients.Group(NombreDelGrupo(mensajeEnviado.IntercambioId))
            .RecibirMensaje(mensajeParaElChat);
    }


    // el id sale del claim NameIdentifier de la cookie, igual que en los controladores
    private int ObtenerIdUsuarioAutenticado()
    {
        string? identificadorUsuario = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(identificadorUsuario, out int idUsuarioAutenticado))
        {
            return 0;
        }

        return idUsuarioAutenticado;
    }

    private static bool EsParticipante(Intercambio intercambio, int idUsuarioAutenticado)
    {
        bool esUsuarioOrigen = intercambio.UsuarioOrigenId == idUsuarioAutenticado;
        bool esUsuarioDestino = intercambio.UsuarioDestinoId == idUsuarioAutenticado;

        return esUsuarioOrigen || esUsuarioDestino;
    }

    private static string NombreDelGrupo(int intercambioId)
    {
        return "intercambio-" + intercambioId;
    }
}
