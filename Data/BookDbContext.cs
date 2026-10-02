using Bookmatch_SL.Models;
using Microsoft.EntityFrameworkCore;

namespace BookmatchSL.Data;

public class BookDbContext : DbContext
{
    public BookDbContext(DbContextOptions<BookDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; }

    public DbSet<Categoria> Categorias { get; set; }

    public DbSet<Libro> Libros { get; set; }

    public DbSet<Intercambio> Intercambios { get; set; }

    public DbSet<Mensaje> Mensajes { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // USO DEL ORM 
    }
}