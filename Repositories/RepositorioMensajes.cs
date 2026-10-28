using Bookmatch_SL.Models;
using Bookmatch_SL.Models.Enums;
using Bookmatch_SL.Repositories.Interfaces;
using BookmatchSL.Data;

using Microsoft.EntityFrameworkCore;

namespace Bookmatch_SL.Repositories;

public class RepositorioMensajes : IRepoMensajes
{
    private readonly BookDbContext contexto_bd;

    public RepositorioMensajes(BookDbContext contexto_bd)
    {
        this.contexto_bd = contexto_bd;
    }


    // alta de un mensaje. Devuelve el id del mensaje nuevo, o 0 si no se pudo guardar.
    // Aunque el controlador ya revisa todo, lo vuelvo a validar acá por si alguien
    // manda la petición directo sin pasar por la pantalla.
    public int Alta(Mensaje nuevo_mensaje)
    {
        if (nuevo_mensaje == null)
        {
            return 0;
        }

        if (string.IsNullOrWhiteSpace(nuevo_mensaje.Contenido))
        {
            return 0;
        }

        Intercambio? intercambio = contexto_bd.Intercambios
            .Find(nuevo_mensaje.IntercambioId);

        if (intercambio == null || !intercambio.Activo)
        {
            return 0;
        }

        // solo pueden escribir los dos usuarios que participan del intercambio
        bool esUsuarioOrigen = intercambio.UsuarioOrigenId == nuevo_mensaje.UsuarioEmisorId;
        bool esUsuarioDestino = intercambio.UsuarioDestinoId == nuevo_mensaje.UsuarioEmisorId;

        if (!esUsuarioOrigen && !esUsuarioDestino)
        {
            return 0;
        }

        // se puede chatear mientras el intercambio está pendiente (para negociar)
        // o aceptado (para coordinar la entrega). Si ya terminó, el chat queda cerrado.
        if (!PermiteMensajes(intercambio))
        {
            return 0;
        }

        nuevo_mensaje.Contenido = nuevo_mensaje.Contenido.Trim();
        // la fecha la pone el servidor, no el usuario, para que no se pueda falsear
        nuevo_mensaje.FechaHora = DateTime.Now;

        contexto_bd.Mensajes.Add(nuevo_mensaje);
        contexto_bd.SaveChanges();

        return nuevo_mensaje.Id;
    }

    // los mensajes no se borran ni se editan: son el historial de lo que se habló
    // en el intercambio, y si se pudieran cambiar se perdería esa información.
    public int Baja(int id)
    {
        throw new NotSupportedException(
            "Los mensajes no se pueden eliminar, forman parte del historial del intercambio."
        );
    }

    public int Modificacion(Mensaje mensaje_a_modificar)
    {
        throw new NotSupportedException(
            "Los mensajes no se pueden editar, forman parte del historial del intercambio."
        );
    }


    // listado general paginado (lo pide la interfaz IRepositorio)
    public IList<Mensaje> ObtenerLista(int nroPagina = 1, int tamPagina = 10)
    {
        if (nroPagina < 1)
        {
            nroPagina = 1;
        }

        if (tamPagina < 1)
        {
            tamPagina = 10;
        }

        int cantidadAOmitir = (nroPagina - 1) * tamPagina;

        return contexto_bd.Mensajes
            .Include(mensaje => mensaje.UsuarioEmisor)
            .OrderBy(mensaje => mensaje.Id)
            .Skip(cantidadAOmitir)
            .Take(tamPagina)
            .ToList();
    }

    public int ObtenerCantidad()
    {
        return contexto_bd.Mensajes.Count();
    }

    public Mensaje? ObtenerPorId(int idMensaje)
    {
        return contexto_bd.Mensajes
            .Include(mensaje => mensaje.UsuarioEmisor)
            .FirstOrDefault(mensaje => mensaje.Id == idMensaje);
    }


    // todos los mensajes de un intercambio, del más viejo al más nuevo,
    // así se leen en orden como en cualquier chat.
    // El Include trae el usuario que lo mandó para poder mostrar su nombre.
    public IList<Mensaje> ObtenerPorIntercambio(int intercambioId)
    {
        return contexto_bd.Mensajes
            .Include(mensaje => mensaje.UsuarioEmisor)
            .Where(mensaje => mensaje.IntercambioId == intercambioId)
            .OrderBy(mensaje => mensaje.FechaHora)
            .ThenBy(mensaje => mensaje.Id)
            .ToList();
    }


    // la regla de cuándo se puede chatear queda en un solo lugar,
    // así la usa el alta y también la vista para saber si muestra el formulario
    public bool PermiteMensajes(Intercambio intercambio)
    {
        return intercambio.Activo &&
               (intercambio.Estado == EstadoIntercambio.Pendiente ||
                intercambio.Estado == EstadoIntercambio.Aceptado);
    }
}
