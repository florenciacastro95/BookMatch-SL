using Bookmatch_SL.Models;
using Bookmatch_SL.Models.Enums;
using Bookmatch_SL.Repositories.Interfaces;
using BookmatchSL.Data;

namespace Bookmatch_SL.Repositories;

public class RepositorioIntercambio : IRepoIntercambio
{
    private readonly BookDbContext contexto_bd;
    public RepositorioIntercambio(BookDbContext contexto_bd)
    {
        this.contexto_bd = contexto_bd;
    }


    //alta
    public int Alta(Intercambio nuevo_intercambio)
    {
        nuevo_intercambio.Activo = true;
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

    //modificaicon
    public int Modificacion(Intercambio intercambio_a_modificar)
    {
        contexto_bd.Intercambios.Update(intercambio_a_modificar);
        return contexto_bd.SaveChanges();
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











}
