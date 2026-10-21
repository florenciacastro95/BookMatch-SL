using Bookmatch_SL.Models;
using Microsoft.EntityFrameworkCore;
using Bookmatch_SL.Models.Enums;



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

        modelBuilder.Entity<Libro>().ToTable("libro");

        modelBuilder.Entity<Libro>()
            .Property(libro => libro.Id)
            .HasColumnName("id");

        modelBuilder.Entity<Libro>()
            .Property(libro => libro.UsuarioId)
            .HasColumnName("usuario_id");

        modelBuilder.Entity<Libro>()
            .Property(libro => libro.CategoriaId)
            .HasColumnName("categoria_id");

        modelBuilder.Entity<Libro>()
            .Property(libro => libro.Titulo)
            .HasColumnName("titulo");

        modelBuilder.Entity<Libro>()
            .Property(libro => libro.Autor)
            .HasColumnName("autor");

        modelBuilder.Entity<Libro>()
            .Property(libro => libro.Editorial)
            .HasColumnName("editorial");

        modelBuilder.Entity<Libro>()
            .Property(libro => libro.Idioma)
            .HasColumnName("idioma");

        modelBuilder.Entity<Libro>()
            .Property(libro => libro.Formato)
            .HasColumnName("formato")
            .HasConversion(
                formato => formato.ToString().Replace("Tapa", "Tapa "),
                texto => Enum.Parse<Formato>(texto.Replace(" ", ""), true)
            );

        modelBuilder.Entity<Libro>()
            .Property(libro => libro.Aclaracion)
            .HasColumnName("aclaracion");

        modelBuilder.Entity<Libro>()
            .Property(libro => libro.PortadaUrl)
            .HasColumnName("portada_url");

        modelBuilder.Entity<Libro>()
            .Property(libro => libro.EstadoConservacion)
            .HasColumnName("estado_conservacion")
            .HasConversion<string>();

        modelBuilder.Entity<Libro>()
            .Property(libro => libro.Disponible)
            .HasColumnName("disponible");

        modelBuilder.Entity<Libro>()
            .Property(libro => libro.Activo)
            .HasColumnName("activo");

        modelBuilder.Entity<Libro>()
            .Property(libro => libro.CreatedAt)
            .HasColumnName("created_at");



    }
}