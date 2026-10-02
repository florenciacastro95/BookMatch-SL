namespace Bookmatch_SL.Models;

public class Categoria
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public bool Activo { get; set; }


    // Relaciones

    public ICollection<Libro> Libros { get; set; } = new List<Libro>();
}