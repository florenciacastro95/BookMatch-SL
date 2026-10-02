namespace Bookmatch_SL.Models;
using Bookmatch_SL.Models.Enums;
public class Libro
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public int CategoriaId { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public string Autor { get; set; } = string.Empty;

    public string Editorial { get; set; } = string.Empty;

    public string Idioma { get; set; } = string.Empty;

    public Formato Formato { get; set; }

    public string? Aclaracion { get; set; }

    public string PortadaUrl { get; set; } = string.Empty;

    public EstadoConservacion EstadoConservacion { get; set; }

    public bool Disponible { get; set; }

    public bool Activo { get; set; }

    public DateTime CreatedAt { get; set; }


  
    public Usuario Usuario { get; set; } = null!;

    public Categoria Categoria { get; set; } = null!;

    public ICollection<Intercambio> IntercambiosComoOfrecido { get; set; }
        = new List<Intercambio>();

    public ICollection<Intercambio> IntercambiosComoSolicitado { get; set; }
        = new List<Intercambio>();
}