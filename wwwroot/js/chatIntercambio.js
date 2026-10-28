// Chat en tiempo real del detalle de un intercambio, usando SignalR.
// Si por algún motivo no se puede conectar, el formulario se manda igual
// de la forma común (POST a MensajeController) y el mensaje no se pierde.

const seccionChat = document.getElementById("chatIntercambio");

if (seccionChat) {
    const intercambioId = parseInt(seccionChat.dataset.intercambioId);
    const idUsuarioAutenticado = parseInt(seccionChat.dataset.usuarioId);

    const listaMensajes = document.getElementById("listaMensajes");
    const avisoSinMensajes = document.getElementById("avisoSinMensajes");
    const formularioMensaje = document.getElementById("formularioMensaje");
    const campoContenido = document.getElementById("contenidoMensaje");
    const textoErrorChat = document.getElementById("errorChat");

    // al abrir la pantalla muestro los últimos mensajes, no los primeros
    listaMensajes.scrollTop = listaMensajes.scrollHeight;

    // la conexión apunta a la misma dirección que pusimos en MapHub de Program.cs
    const conexionChat = new signalR.HubConnectionBuilder()
        .withUrl("/chatIntercambioHub")
        .withAutomaticReconnect()
        .build();

    // "on" = escuchar lo que manda el servidor. El nombre tiene que ser
    // igual al método de IClienteChatIntercambio
    conexionChat.on("RecibirMensaje", function (mensajeChat) {
        agregarMensajeALaLista(mensajeChat);
    });

    conexionChat.on("RecibirError", function (descripcionError) {
        textoErrorChat.textContent = descripcionError;
    });

    // si se corta y se reconecta, la conexión es nueva y no está en ningún grupo,
    // así que hay que volver a entrar a la sala del intercambio
    conexionChat.onreconnected(function () {
        conexionChat.invoke("UnirseAlChat", intercambioId);
    });

    conexionChat.start()
        .then(function () {
            // "invoke" = ejecutar un método del hub en el servidor
            return conexionChat.invoke("UnirseAlChat", intercambioId);
        })
        .catch(function (errorConexion) {
            console.error("No se pudo conectar al chat en tiempo real", errorConexion);
        });

    if (formularioMensaje) {
        formularioMensaje.addEventListener("submit", function (eventoEnvio) {
            // sin conexión dejo que el formulario se mande normal (recarga la página)
            if (conexionChat.state !== signalR.HubConnectionState.Connected) {
                return;
            }

            eventoEnvio.preventDefault();

            const contenidoEscrito = campoContenido.value.trim();
            if (contenidoEscrito === "") {
                textoErrorChat.textContent = "El mensaje no puede estar vacío";
                return;
            }

            // mando un objeto (el DTO MensajeEnviadoDto) y no datos sueltos
            conexionChat.invoke("EnviarMensaje", {
                intercambioId: intercambioId,
                contenido: contenidoEscrito
            })
                .then(function () {
                    campoContenido.value = "";
                    textoErrorChat.textContent = "";
                })
                .catch(function (errorEnvio) {
                    console.error(errorEnvio);
                    textoErrorChat.textContent = "No se pudo enviar el mensaje";
                });
        });
    }

    // arma la "burbuja" del mensaje igual que la que dibuja _Mensajes.cshtml
    function agregarMensajeALaLista(mensajeChat) {
        if (avisoSinMensajes) {
            avisoSinMensajes.remove();
        }

        // se compara por id y no por nombre, porque puede haber dos usuarios con el mismo nombre
        const esMio = mensajeChat.usuarioEmisorId === idUsuarioAutenticado;

        const filaMensaje = document.createElement("div");
        filaMensaje.className = "d-flex mb-2 " + (esMio ? "justify-content-end" : "justify-content-start");

        const burbujaMensaje = document.createElement("div");
        burbujaMensaje.className = "border rounded px-3 py-2" + (esMio ? " bg-light" : "");
        burbujaMensaje.style.maxWidth = "75%";

        const nombreEmisor = document.createElement("small");
        nombreEmisor.className = "d-block fw-bold";
        nombreEmisor.textContent = esMio ? "Vos" : mensajeChat.nombreEmisor;

        // textContent y no innerHTML: así si alguien escribe código HTML se muestra como texto
        const contenidoMensaje = document.createElement("span");
        contenidoMensaje.style.whiteSpace = "pre-line";
        contenidoMensaje.textContent = mensajeChat.contenido;

        const fechaMensaje = document.createElement("small");
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
}
