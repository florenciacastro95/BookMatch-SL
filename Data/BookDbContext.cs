using Bookmatch_SL.Models;
using Microsoft.EntityFrameworkCore;
using Bookmatch_SL.Models.Enums;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;


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

    public DbSet<Auditoria> Auditorias { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //USUARIO
        modelBuilder.Entity<Usuario>()
               .ToTable("usuario");

        modelBuilder.Entity<Usuario>()
            .Property(usuario => usuario.Id)
            .HasColumnName("id");

        modelBuilder.Entity<Usuario>()
            .Property(usuario => usuario.Nombre)
            .HasColumnName("nombre");

        modelBuilder.Entity<Usuario>()
            .Property(usuario => usuario.Email)
            .HasColumnName("email");

        modelBuilder.Entity<Usuario>()
            .Property(usuario => usuario.PasswordHash)
            .HasColumnName("password_hash");

        modelBuilder.Entity<Usuario>()
            .Property(usuario => usuario.Rol)
            .HasColumnName("rol")
            .HasConversion<string>();

        modelBuilder.Entity<Usuario>()
            .Property(usuario => usuario.AvatarUrl)
            .HasColumnName("avatar_url");

        modelBuilder.Entity<Usuario>()
            .Property(usuario => usuario.Activo)
            .HasColumnName("activo");

        modelBuilder.Entity<Usuario>()
            .Property(usuario => usuario.CreatedAt)
            .HasColumnName("created_at");

        modelBuilder.Entity<Usuario>()
            .HasIndex(usuario => usuario.Email)
            .IsUnique();


        //CAT

        modelBuilder.Entity<Categoria>()
            .ToTable("categoria");

        modelBuilder.Entity<Categoria>()
            .Property(categoria => categoria.Id)
            .HasColumnName("id");

        modelBuilder.Entity<Categoria>()
            .Property(categoria => categoria.Nombre)
            .HasColumnName("nombre");

        modelBuilder.Entity<Categoria>()
            .Property(categoria => categoria.Activo)
            .HasColumnName("activo");

        //LIBRO

        modelBuilder.Entity<Libro>()
            .ToTable("libro");

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

        //relaciones de ibro que faltaban

        modelBuilder.Entity<Libro>()
            .HasOne(libro => libro.Usuario)
            .WithMany(usuario => usuario.Libros)
            .HasForeignKey(libro => libro.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Libro>()
            .HasOne(libro => libro.Categoria)
            .WithMany(categoria => categoria.Libros)
            .HasForeignKey(libro => libro.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);


        // INTERCAMBIO

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

        // Nombres de tabla y columnas de Intercambio

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


        // MENSAJE

        modelBuilder.Entity<Mensaje>()
            .ToTable("mensaje");

        modelBuilder.Entity<Mensaje>()
            .Property(mensaje => mensaje.Id)
            .HasColumnName("id");

        modelBuilder.Entity<Mensaje>()
            .Property(mensaje => mensaje.IntercambioId)
            .HasColumnName("intercambio_id");

        modelBuilder.Entity<Mensaje>()
            .Property(mensaje => mensaje.UsuarioEmisorId)
            .HasColumnName("usuario_emisor_id");

        modelBuilder.Entity<Mensaje>()
            .Property(mensaje => mensaje.Contenido)
            .HasColumnName("contenido");

        modelBuilder.Entity<Mensaje>()
            .Property(mensaje => mensaje.FechaHora)
            .HasColumnName("fecha_hora");

        // Relaciones de Mensaje

        modelBuilder.Entity<Mensaje>()
            .HasOne(mensaje => mensaje.Intercambio)
            .WithMany(intercambio => intercambio.Mensajes)
            .HasForeignKey(mensaje => mensaje.IntercambioId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Mensaje>()
            .HasOne(mensaje => mensaje.UsuarioEmisor)
            .WithMany(usuario => usuario.MensajesEnviados)
            .HasForeignKey(mensaje => mensaje.UsuarioEmisorId)
            .OnDelete(DeleteBehavior.Restrict);

        // AUDITORIA

        modelBuilder.Entity<Auditoria>()
            .ToTable("auditoria");

        modelBuilder.Entity<Auditoria>()
            .HasKey(auditoria => auditoria.Id);

        modelBuilder.Entity<Auditoria>()
            .Property(auditoria => auditoria.Id)
            .HasColumnName("id_auditoria");

        modelBuilder.Entity<Auditoria>()
            .Property(auditoria => auditoria.IdUsuario)
            .HasColumnName("id_usuario");

        modelBuilder.Entity<Auditoria>()
            .Property(auditoria => auditoria.Accion)
            .HasColumnName("accion")
            .HasMaxLength(100);

        modelBuilder.Entity<Auditoria>()
            .Property(auditoria => auditoria.Entidad)
            .HasColumnName("entidad")
            .HasMaxLength(100);

        modelBuilder.Entity<Auditoria>()
            .Property(auditoria => auditoria.IdEntidad)
            .HasColumnName("id_entidad");

        modelBuilder.Entity<Auditoria>()
            .Property(auditoria => auditoria.FechaHora)
            .HasColumnName("fecha_hora");

        modelBuilder.Entity<Auditoria>()
            .Property(auditoria => auditoria.DatosAnteriores)
            .HasColumnName("datos_anteriores");

        modelBuilder.Entity<Auditoria>()
            .Property(auditoria => auditoria.DatosNuevos)
            .HasColumnName("datos_nuevos");
    }
}