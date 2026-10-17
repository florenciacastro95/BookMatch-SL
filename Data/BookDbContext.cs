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

        modelBuilder.Entity<Intercambio>()
            .HasOne(intercambio => intercambio.UsuarioOrigen)
            .WithMany(usuario => usuario.IntercambiosOrigen)
            .HasForeignKey(intercambio => intercambio.UsuarioOrigenId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Intercambio>()
            .HasOne(intercambio => intercambio.UsuarioDestino)
            .WithMany(usuario => usuario.IntercambiosDestino)
            .HasForeignKey(intercambio => intercambio.UsuarioDestinoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Intercambio>()
            .HasOne(intercambio => intercambio.LibroOfrecido)
            .WithMany(libro => libro.IntercambiosComoOfrecido)
            .HasForeignKey(intercambio => intercambio.LibroOfrecidoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Intercambio>()
            .HasOne(intercambio => intercambio.LibroSolicitado)
            .WithMany(libro => libro.IntercambiosComoSolicitado)
            .HasForeignKey(intercambio => intercambio.LibroSolicitadoId)
            .OnDelete(DeleteBehavior.Restrict);
        // relaciones tabla y col correspondientes al modelo Interca
        modelBuilder.Entity<Intercambio>()
            .ToTable("intercambio");

        modelBuilder.Entity<Intercambio>()
            .Property(intercambio => intercambio.Id)
            .HasColumnName("id");

        modelBuilder.Entity<Intercambio>()
            .Property(intercambio => intercambio.UsuarioOrigenId)
            .HasColumnName("usuario_origen_id");

        modelBuilder.Entity<Intercambio>()
            .Property(intercambio => intercambio.UsuarioDestinoId)
            .HasColumnName("usuario_destino_id");


        modelBuilder.Entity<Intercambio>()
            .Property(intercambio => intercambio.LibroOfrecidoId)
            .HasColumnName("libro_ofrecido_id");

        modelBuilder.Entity<Intercambio>()
            .Property(intercambio => intercambio.LibroSolicitadoId)
            .HasColumnName("libro_solicitado_id");

        modelBuilder.Entity<Intercambio>()
            .Property(intercambio => intercambio.Estado)
            .HasColumnName("estado")
            .HasConversion<string>();

        modelBuilder.Entity<Intercambio>()
            .Property(intercambio => intercambio.Activo)
            .HasColumnName("activo");

        modelBuilder.Entity<Intercambio>()
            .Property(intercambio => intercambio.CreatedAt)
            .HasColumnName("created_at");



    }
}