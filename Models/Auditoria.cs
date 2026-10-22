
namespace Bookmatch_SL.Models;

public class Auditoria
{
    public int Id { get; set; }

    // el IdUsuario es quien realizó la acción. Puede ser null si no se pudo identificar al usuario.
    public int? IdUsuario { get; set; }

    // Ejemplos: ALTA, MODIFICACION, BAJA_LOGICA, ACEPTACION_INTERCAMBIO, INICIO_SESION.
    public string Accion { get; set; } = string.Empty;

    // Ejemplos: Usuario, Libro, Intercambio.
    public string Entidad { get; set; } = string.Empty;

    // Identificador del registro afectado.
    public int? IdEntidad { get; set; }

    public DateTime FechaHora { get; set; }

    // Datos previos al cambio, en formato JSON.
    public string? DatosAnteriores { get; set; }

    // Datos posteriores al cambio, en formato JSON.
    public string? DatosNuevos { get; set; }
}
