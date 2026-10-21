using Bookmatch_SL.Models;
using Bookmatch_SL.Models.Enums;
using Bookmatch_SL.Repositories.Interfaces;
using BookmatchSL.Data;

using Microsoft.EntityFrameworkCore;

namespace Bookmatch_SL.Repositories;

public class RepositorioIntercambio : IRepoIntercambio
{
    private readonly BookDbContext contexto_bd;
    public RepositorioIntercambio(BookDbContext contexto_bd)
    {
        this.contexto_bd = contexto_bd;
    }


    //alta, solicitud=pendiente, libros siguen disponibles hasta que se el ok del intermcabio 
    public int Alta(Intercambio nuevo_intercambio)
    {
        if (nuevo_intercambio.UsuarioOrigenId <= 0 || nuevo_intercambio.UsuarioDestinoId <= 0)
        {
            return 0;
        }

        if (nuevo_intercambio.UsuarioOrigenId == nuevo_intercambio.UsuarioDestinoId)
        {
            return 0;
        }

        if (nuevo_intercambio.LibroOfrecidoId == nuevo_intercambio.LibroSolicitadoId)
        {
            return 0;
        }

        Libro? libroOfrecido = contexto_bd.Libros
            .Find(nuevo_intercambio.LibroOfrecidoId);

        Libro? libroSolicitado = contexto_bd.Libros
            .Find(nuevo_intercambio.LibroSolicitadoId);

        if (libroOfrecido == null || libroSolicitado == null)
        {
            return 0;
        }

        if (!libroOfrecido.Activo || !libroSolicitado.Activo)
        {
            return 0;
        }

        if (!libroOfrecido.Disponible || !libroSolicitado.Disponible)
        {
            return 0;
        }

        if (libroOfrecido.UsuarioId != nuevo_intercambio.UsuarioOrigenId)
        {
            return 0;
        }

        if (libroSolicitado.UsuarioId != nuevo_intercambio.UsuarioDestinoId)
        {
            return 0;
        }

        nuevo_intercambio.Estado = EstadoIntercambio.Pendiente;
        nuevo_intercambio.Activo = true;
        nuevo_intercambio.CreatedAt = DateTime.Now;

        contexto_bd.Intercambios.Add(nuevo_intercambio);
        contexto_bd.SaveChanges();

        return nuevo_intercambio.Id;
    }

    //bja
    public int Baja(int intercambio_id)
    {
        var intercambio_encontrado =
            contexto_bd.Intercambios.Find(intercambio_id);

        if (intercambio_encontrado == null)
        {
            return 0;
        }
        intercambio_encontrado.Activo = false;
        return contexto_bd.SaveChanges();
    }

    //modificaicon,user no pueden modificar, pero implementado en IRepo
    public int Modificacion(Intercambio intercambio_a_modificar)
    {
        throw new NotSupportedException(
        "El intercambio no admite edición general. Utilice Aceptar, Rechazar, Cancelar o MarcarExitoso."
    );
    }

    // listado y  paginado-

    public IList<Intercambio> ObtenerLista(int nroPagina = 1, int tamPagina = 10)
    {

        if (nroPagina < 1)
        {
            nroPagina = 1;
        }

        if (tamPagina < 1)
        {
            tamPagina = 10;
        }
        // ej pag 2 con 10 registro, se omiten los primeros 10 y devuelve los sig 10
        // (2 -1 ) * 10 = 10 sig
        int cantidadAOmitir = (nroPagina - 1) * tamPagina;

        return contexto_bd.Intercambios
            .Include(intercambio => intercambio.LibroOfrecido)
.Include(intercambio => intercambio.LibroSolicitado)
            .Where(intercambio => intercambio.Activo)
            .OrderBy(intercambio => intercambio.Id)
            .Skip(cantidadAOmitir)
            .Take(tamPagina)
            .ToList();
    }



    public int ObtenerCantidad()
    {
        return contexto_bd.Intercambios
            .Count(intercambio => intercambio.Activo);
    }
    // permisivo pra devovler nill o el intercam x id
    public Intercambio? ObtenerPorId(int idIntercambio)
    {
        return contexto_bd.Intercambios
         .Include(intercambio => intercambio.LibroOfrecido)
        .Include(intercambio => intercambio.LibroSolicitado)
            .FirstOrDefault(intercambio =>
                intercambio.Id == idIntercambio && intercambio.Activo);
    }

    // - intermbios activos donde el user participa como origen o destino

    public IList<Intercambio> ObtenerPorUsuario(int usuarioId)
    {
        return contexto_bd.Intercambios
            .Where(intercambio =>
                intercambio.Activo &&
                (intercambio.UsuarioOrigenId == usuarioId ||
                 intercambio.UsuarioDestinoId == usuarioId))
            .OrderBy(intercambio => intercambio.Id)
            .ToList();
    }

    //intermcabios pendientes de todos los user
    public IList<Intercambio> ObtenerPorEstado(EstadoIntercambio estado)
    {
        return contexto_bd.Intercambios
            .Where(intercambio =>
                intercambio.Activo && intercambio.Estado == estado)
            .OrderBy(intercambio => intercambio.Id)
            .ToList();
    }


    public int Rechazar(int idIntercambio, int idUsuarioAutenticado)
    {
        Intercambio? solicitudIntercambio = ObtenerPorId(idIntercambio);

        if (solicitudIntercambio == null)
        {
            return 0;
        }

        if (solicitudIntercambio.UsuarioDestinoId != idUsuarioAutenticado)
        {
            return 0;
        }

        if (!solicitudIntercambio.PuedeCambiarA(EstadoIntercambio.Rechazado))
        {
            return 0;
        }

        solicitudIntercambio.Estado = EstadoIntercambio.Rechazado;

        return contexto_bd.SaveChanges();
    }


    public int Aceptar(int idIntercambio, int idUsuarioAutenticado)
    {
        Intercambio? solicitudIntercambio = ObtenerPorId(idIntercambio);

        if (solicitudIntercambio == null)
        {
            return 0;
        }

        if (solicitudIntercambio.UsuarioDestinoId != idUsuarioAutenticado)
        {
            return 0;
        }

        if (!solicitudIntercambio.PuedeCambiarA(EstadoIntercambio.Aceptado))
        {
            return 0;
        }

        if (solicitudIntercambio.UsuarioOrigenId == solicitudIntercambio.UsuarioDestinoId)
        {
            return 0;
        }

        if (solicitudIntercambio.LibroOfrecidoId == solicitudIntercambio.LibroSolicitadoId)
        {
            return 0;
        }

        Libro? libroOfrecido = contexto_bd.Libros
            .Find(solicitudIntercambio.LibroOfrecidoId);

        Libro? libroSolicitado = contexto_bd.Libros
            .Find(solicitudIntercambio.LibroSolicitadoId);

        if (libroOfrecido == null || libroSolicitado == null)
        {
            return 0;
        }

        if (!libroOfrecido.Activo || !libroSolicitado.Activo)
        {
            return 0;
        }

        if (!libroOfrecido.Disponible || !libroSolicitado.Disponible)
        {
            return 0;
        }

        if (libroOfrecido.UsuarioId != solicitudIntercambio.UsuarioOrigenId)
        {
            return 0;
        }

        if (libroSolicitado.UsuarioId != solicitudIntercambio.UsuarioDestinoId)
        {
            return 0;
        }

        solicitudIntercambio.Estado = EstadoIntercambio.Aceptado;
        libroOfrecido.Disponible = false;
        libroSolicitado.Disponible = false;

        return contexto_bd.SaveChanges();
    }



    public int Cancelar(int idIntercambio, int idUsuarioAutenticado)
    {
        Intercambio? solicitudIntercambio = ObtenerPorId(idIntercambio);
        if (solicitudIntercambio == null)
        {
            return 0;
        }

        bool esUsuarioOrigen = solicitudIntercambio.UsuarioOrigenId == idUsuarioAutenticado;
        bool esUsuarioDestino = solicitudIntercambio.UsuarioDestinoId == idUsuarioAutenticado;

        if (!esUsuarioOrigen && !esUsuarioDestino)
        {
            return 0;
        }

        if (!solicitudIntercambio.PuedeCambiarA(EstadoIntercambio.Cancelado))
        {
            return 0;
        }

        if (solicitudIntercambio.Estado == EstadoIntercambio.Aceptado)
        {
            Libro? libroOfrecido = contexto_bd.Libros
                .Find(solicitudIntercambio.LibroOfrecidoId);

            Libro? libroSolicitado = contexto_bd.Libros
                .Find(solicitudIntercambio.LibroSolicitadoId);

            if (libroOfrecido == null || libroSolicitado == null)
            {
                return 0;
            }
            libroOfrecido.Disponible = true;
            libroSolicitado.Disponible = true;
        }

        solicitudIntercambio.Estado = EstadoIntercambio.Cancelado;
        return contexto_bd.SaveChanges();
    }

    public int MarcarExitoso(int idIntercambio, int idUsuarioAutenticado)
    {
        Intercambio? solicitudIntercambio = ObtenerPorId(idIntercambio);

        if (solicitudIntercambio == null)
        {
            return 0;
        }

        bool esUsuarioOrigen = solicitudIntercambio.UsuarioOrigenId == idUsuarioAutenticado;

        bool esUsuarioDestino = solicitudIntercambio.UsuarioDestinoId == idUsuarioAutenticado;

        if (!esUsuarioOrigen && !esUsuarioDestino)
        {
            return 0;
        }

        if (!solicitudIntercambio.PuedeCambiarA(EstadoIntercambio.Exitoso))
        {
            return 0;
        }

        solicitudIntercambio.Estado = EstadoIntercambio.Exitoso;


        return contexto_bd.SaveChanges();
    }

    public IList<Intercambio> ObtenerEnviados(int usuarioId)
    {
        return contexto_bd.Intercambios
            .Where(solicitudIntercambio =>
                solicitudIntercambio.Activo &&
                solicitudIntercambio.UsuarioOrigenId == usuarioId)
            .OrderBy(solicitudIntercambio => solicitudIntercambio.Id)
            .ToList();
    }

    public IList<Intercambio> ObtenerRecibidos(int usuarioId)
    {
        return contexto_bd.Intercambios
            .Where(solicitudIntercambio =>
                solicitudIntercambio.Activo &&
                solicitudIntercambio.UsuarioDestinoId == usuarioId)
            .OrderBy(solicitudIntercambio => solicitudIntercambio.Id)
            .ToList();
    }







}
