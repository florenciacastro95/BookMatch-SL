"use strict";

// Chat en tiempo real del detalle de un intercambio, usando SignalR
// (misma forma que el chat.js del ejemplo de clase).
// intercambioId e idUsuarioAutenticado los define Detalle.cshtml en la sección Scripts.
// Si no se puede conectar, el formulario se manda igual de la forma común
// (POST a MensajeController) y el mensaje no se pierde.

var listaMensajes = document.getElementById("listaMensajes");
var avisoSinMensajes = document.getElementById("avisoSinMensajes");
var formularioMensaje = document.getElementById("formularioMensaje");
var campoContenido = document.getElementById("contenidoMensaje");
var textoErrorChat = document.getElementById("errorChat");

// al abrir la pantalla muestro los últimos mensajes, no los primeros
listaMensajes.scrollTop = listaMensajes.scrollHeight;

// la conexión apunta a la misma dirección que pusimos en MapHub de Program.cs
var conexionChat = new signalR.HubConnectionBuilder().withUrl("/chatIntercambioHub").build();

// "on" = escuchar lo que manda el servidor. El nombre tiene que ser
// igual al método de IClienteChatIntercambio
conexionChat.on("RecibirMensaje", function (mensajeChat) {
    // el otro usuario puede tener abierto otro intercambio conmigo,
    // así que solo muestro los mensajes de este intercambio
    if (mensajeChat.intercambioId !== intercambioId) {
        return;
    }
    agregarMensajeALaLista(mensajeChat);
});

conexionChat.on("RecibirError", function (descripcionError) {
    textoErrorChat.textContent = descripcionError;
});

conexionChat.start().catch(function (errorConexion) {
    return console.error(errorConexion.toString());
});

if (formularioMensaje) {
    formularioMensaje.addEventListener("submit", function (eventoEnvio) {
        // sin conexión dejo que el formulario se mande normal (recarga la página)
        if (conexionChat.state !== signalR.HubConnectionState.Connected) {
            return;
        }

        eventoEnvio.preventDefault();

        var contenidoEscrito = campoContenido.value.trim();
        if (contenidoEscrito === "") {
            textoErrorChat.textContent = "El mensaje no puede estar vacío";
            return;
        }

        // "invoke" = ejecutar un método del hub en el servidor.
        // Mando un objeto (el DTO MensajeEnviadoDto) y no datos sueltos
        conexionChat.invoke("EnviarMensaje", {
            intercambioId: intercambioId,
            contenido: contenidoEscrito
        }).then(function () {
            campoContenido.value = "";
            textoErrorChat.textContent = "";
        }).catch(function (errorEnvio) {
            textoErrorChat.textContent = "No se pudo enviar el mensaje";
            return console.error(errorEnvio.toString());
        });
    });
}

// arma la "burbuja" del mensaje igual que la que dibuja _Mensajes.cshtml
function agregarMensajeALaLista(mensajeChat) {
    if (avisoSinMensajes) {
        avisoSinMensajes.remove();
        avisoSinMensajes = null;
    }

    // se compara por id y no por nombre, como en el chat seguro del profe
    var esMio = mensajeChat.usuarioEmisorId === idUsuarioAutenticado;

    var filaMensaje = document.createElement("div");
    filaMensaje.className = "d-flex mb-2 " + (esMio ? "justify-content-end" : "justify-content-start");

    var burbujaMensaje = document.createElement("div");
    burbujaMensaje.className = "border rounded px-3 py-2" + (esMio ? " bg-light" : "");
    burbujaMensaje.style.maxWidth = "75%";

    var nombreEmisor = document.createElement("small");
    nombreEmisor.className = "d-block fw-bold";
    nombreEmisor.textContent = esMio ? "Vos" : mensajeChat.nombreEmisor;

    // textContent y no innerHTML: así si alguien escribe código HTML se muestra como texto
    var contenidoMensaje = document.createElement("span");
    contenidoMensaje.style.whiteSpace = "pre-line";
    contenidoMensaje.textContent = mensajeChat.contenido;

    var fechaMensaje = document.createElement("small");
    fechaMensaje.className = "d-block text-end";
    fechaMensaje.textContent = mensajeChat.fechaHoraTexto;

    burbujaMensaje.appendChild(nombreEmisor);
    burbujaMensaje.appendChild(contenidoMensaje);
    burbujaMensaje.appendChild(fechaMensaje);
    filaMensaje.appendChild(burbujaMensaje);
    listaMensajes.appendChild(filaMensaje);

    // bajo hasta el último mensaje, como en cualquier chat
    listaMensajes.scrollTop = listaMensajes.scrollHeight;
}
