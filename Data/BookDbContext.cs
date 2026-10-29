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
    private readonly IHttpContextAccessor accesoHttp;

    public BookDbContext(
        DbContextOptions<BookDbContext> options,
        IHttpContextAccessor accesoHttp)
        : base(options)
    {
        this.accesoHttp = accesoHttp;
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

    private static readonly JsonSerializerOptions OpcionesJsonAuditoria = new()
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private int? ObtenerIdUsuarioActual()
    {
        string? valorId = accesoHttp.HttpContext?
            .User
            .FindFirstValue(ClaimTypes.NameIdentifier);

        return int.TryParse(valorId, out int id)
            ? id
            : null;
    }

    private static bool EsPropiedadSensible(string nombre)
    {
        return nombre.Contains(
                   "Password",
                   StringComparison.OrdinalIgnoreCase)
               || nombre.Contains(
                   "Token",
                   StringComparison.OrdinalIgnoreCase);
    }

    private static int? ObtenerIdEntidad(EntityEntry entrada)
    {
        var propiedadClave = entrada.Metadata
            .FindPrimaryKey()?
            .Properties
            .FirstOrDefault();

        if (propiedadClave == null)
            return null;

        var valor = entrada.Property(propiedadClave.Name).CurrentValue;

        if (valor == null)
            return null;

        return int.TryParse(valor.ToString(), out int id)
            ? id
            : null;
    }

    private static string SerializarValores(
        EntityEntry entrada,
        bool originales,
        bool soloModificados)
    {
        var valores = new Dictionary<string, object?>();

        var nombresClave = entrada.Metadata.FindPrimaryKey()?
            .Properties
            .Select(p => p.Name)
            .ToHashSet()
            ?? new HashSet<string>();

        foreach (var propiedad in entrada.Properties)
        {
            string nombre = propiedad.Metadata.Name;

            // No guardamos contraseñas, hashes ni tokens.
            if (EsPropiedadSensible(nombre))
                continue;

            if (soloModificados && !propiedad.IsModified)
                continue;

            // El ID autogenerado todavía no existe al capturar
            // una entidad que se está creando.
            if (entrada.State == EntityState.Added &&
                nombresClave.Contains(nombre))
            {
                continue;
            }

            valores[nombre] = originales
                ? propiedad.OriginalValue
                : propiedad.CurrentValue;
        }

        return JsonSerializer.Serialize(valores, OpcionesJsonAuditoria);
    }

    private string DeterminarAccion(EntityEntry entrada)
    {
        if (entrada.Entity is Usuario &&
            entrada.State == EntityState.Modified &&
            entrada.Property(nameof(Usuario.PasswordHash)).IsModified)
        {
            return "CAMBIO_PASSWORD";
        }

        if (entrada.State == EntityState.Modified &&
            entrada.Properties.Any(p =>
                p.Metadata.Name == "Activo" &&
                p.IsModified &&
                Equals(p.OriginalValue, true) &&
                Equals(p.CurrentValue, false)))
        {
            return "BAJA_LOGICA";
        }
        if (entrada.Entity is Libro &&
            entrada.State == EntityState.Modified &&
            entrada.Property(nameof(Libro.Disponible)).IsModified)
        {
            return "CAMBIO_DISPONIBILIDAD";
        }
        if (entrada.Entity is Intercambio &&
            entrada.State == EntityState.Modified &&
            entrada.Property(nameof(Intercambio.Estado)).IsModified)
        {
            return "CAMBIO_ESTADO_INTERCAMBIO";
        }

        return entrada.State switch
        {
            EntityState.Added => "ALTA",
            EntityState.Modified => "MODIFICACION",
            EntityState.Deleted => "BAJA_FISICA",
            _ => "OTRA"
        };
    }

    private List<(EntityEntry Entrada, Auditoria Registro)>
        PrepararRegistrosAuditoria()
    {
        ChangeTracker.DetectChanges();

        var entradas = ChangeTracker.Entries()
            .Where(e =>
                e.Entity is not Auditoria &&
                e.State is EntityState.Added
                    or EntityState.Modified
                    or EntityState.Deleted)
            .ToList();

        var registros =
            new List<(EntityEntry Entrada, Auditoria Registro)>();

        foreach (var entrada in entradas)
        {
            bool esNueva = entrada.State == EntityState.Added;
            bool esModificada = entrada.State == EntityState.Modified;
            bool esEliminada = entrada.State == EntityState.Deleted;

            var registro = new Auditoria
            {
                IdUsuario = ObtenerIdUsuarioActual(),
                Accion = DeterminarAccion(entrada),
                Entidad = entrada.Metadata.ClrType.Name,
                IdEntidad = esNueva ? null : ObtenerIdEntidad(entrada),
                FechaHora = DateTime.UtcNow,

                DatosAnteriores = esModificada || esEliminada
                    ? SerializarValores(
                        entrada,
                        originales: esModificada,
                        soloModificados: esModificada)
                    : null,

                DatosNuevos = esNueva || esModificada
                    ? SerializarValores(
                        entrada,
                        originales: false,
                        soloModificados: esModificada)
                    : null
            };

            registros.Add((entrada, registro));
        }

        return registros;
    }

    public override int SaveChanges()
    {
        var registros = PrepararRegistrosAuditoria();

        // Si solo se están guardando registros de auditoría,
        // no necesitamos generar otros registros de auditoría.
        if (registros.Count == 0)
            return base.SaveChanges();

        IDbContextTransaction? transaccion = null;

        if (Database.IsRelational() &&
            Database.CurrentTransaction == null)
        {
            transaccion = Database.BeginTransaction();
        }

        try
        {
            // Primero guardamos las entidades reales.
            int cantidadAfectada = base.SaveChanges();

            // Una vez guardadas, las entidades nuevas ya tienen ID.
            foreach (var (entrada, registro) in registros)
            {
                if (registro.IdEntidad == null)
                    registro.IdEntidad = ObtenerIdEntidad(entrada);

                Auditorias.Add(registro);
            }

            // El método base evita volver a ejecutar esta lógica
            // de auditoría al guardar sus propios registros.
            base.SaveChanges();

            transaccion?.Commit();

            return cantidadAfectada;
        }
        catch
        {
            transaccion?.Rollback();
            throw;
        }
        finally
        {
            transaccion?.Dispose();
        }
    }

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        var registros = PrepararRegistrosAuditoria();

        if (registros.Count == 0)
            return await base.SaveChangesAsync(cancellationToken);

        IDbContextTransaction? transaccion = null;

        if (Database.IsRelational() &&
            Database.CurrentTransaction == null)
        {
            transaccion = await Database.BeginTransactionAsync(
                cancellationToken);
        }

        try
        {
            int cantidadAfectada =
                await base.SaveChangesAsync(cancellationToken);

            foreach (var (entrada, registro) in registros)
            {
                if (registro.IdEntidad == null)
                    registro.IdEntidad = ObtenerIdEntidad(entrada);

                Auditorias.Add(registro);
            }

            await base.SaveChangesAsync(cancellationToken);

            if (transaccion != null)
            {
                await transaccion.CommitAsync(cancellationToken);
            }

            return cantidadAfectada;
        }
        catch
        {
            if (transaccion != null)
                await transaccion.RollbackAsync(CancellationToken.None);

            throw;
        }
        finally
        {
            if (transaccion != null)
                await transaccion.DisposeAsync();
        }
    }
}