using System.Security.Claims;
using System.Text;
using Bookmatch_SL.Hubs;
using Bookmatch_SL.Models;
using Bookmatch_SL.Repositories;
using Bookmatch_SL.Repositories.Interfaces;
using Bookmatch_SL.Services;
using BookmatchSL.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
// SignalR para el chat en tiempo real de los intercambios
builder.Services.AddSignalR();
builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();


builder.Services.AddScoped<IRepoIntercambio, RepositorioIntercambio>();
builder.Services.AddScoped<IRepoLibro, RepositorioLibro>();
builder.Services.AddScoped<IRepoCategoria, RepositorioCategoria>();
builder.Services.AddScoped<IRepoUsuarios, RepositorioUsuario>();
builder.Services.AddScoped<IRepoMensajes, RepositorioMensajes>();


builder.Services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();
builder.Services.AddScoped<ServicioAvatar>();
builder.Services.AddScoped<ServicioPortada>();

string conexionBaseDatos = builder.Configuration
    .GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Falta configurar ConnectionStrings:DefaultConnection en User Secrets.");

builder.Services.AddDbContext<BookDbContext>(opciones =>
    opciones.UseMySql(
        conexionBaseDatos,
        ServerVersion.AutoDetect(conexionBaseDatos)
    )
);

string jwtClave = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException(
        "Falta configurar Jwt:Key en User Secrets.");

if (Encoding.UTF8.GetByteCount(jwtClave) < 32)
{
    throw new InvalidOperationException(
        "Jwt:Key debe tener como mínimo 32 bytes.");
}

string jwtEmisor = builder.Configuration["Jwt:Issuer"]
    ?? "BookmatchSL";

string jwtAudiencia = builder.Configuration["Jwt:Audience"]
    ?? "BookmatchSL";

var claveFirma = new SymmetricSecurityKey(
    Encoding.UTF8.GetBytes(jwtClave));

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            CookieAuthenticationDefaults.AuthenticationScheme;

        options.DefaultSignInScheme =
            CookieAuthenticationDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(
        CookieAuthenticationDefaults.AuthenticationScheme,
        options =>
        {
            options.LoginPath = "/Auth/Login";
            options.AccessDeniedPath = "/Auth/Denegado";

            options.Cookie.Name = "BookmatchSL.Auth";
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Lax;

            options.ExpireTimeSpan = TimeSpan.FromHours(8);
            options.SlidingExpiration = true;

            options.Events.OnValidatePrincipal = async contexto =>
            {
                var valorId = contexto.Principal?
                    .FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!int.TryParse(valorId, out int idUsuario))
                {
                    contexto.RejectPrincipal();

                    await contexto.HttpContext.SignOutAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme);

                    return;
                }

                var repo = contexto.HttpContext.RequestServices
                    .GetRequiredService<IRepoUsuarios>();

                var usuario = repo.ObtenerPorId(idUsuario);

                if (usuario == null || !usuario.Activo)
                {
                    contexto.RejectPrincipal();

                    await contexto.HttpContext.SignOutAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme);

                    return;
                }

                string? nombreCookie = contexto.Principal?
                    .FindFirst(ClaimTypes.Name)?.Value;

                string? emailCookie = contexto.Principal?
                    .FindFirst(ClaimTypes.Email)?.Value;

                string? rolCookie = contexto.Principal?
                    .FindFirst(ClaimTypes.Role)?.Value;

                if (nombreCookie != usuario.Nombre ||
                    emailCookie != usuario.Email ||
                    rolCookie != usuario.Rol.ToString())
                {
                    var claims = new List<Claim>
                    {
                        new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                        new(ClaimTypes.Name, usuario.Nombre),
                        new(ClaimTypes.Email, usuario.Email),
                        new(ClaimTypes.Role, usuario.Rol.ToString())
                    };

                    var identidad = new ClaimsIdentity(
                        claims,
                        CookieAuthenticationDefaults.AuthenticationScheme);

                    contexto.ReplacePrincipal(
                        new System.Security.Claims.ClaimsPrincipal(identidad));

                    contexto.ShouldRenew = true;
                }
            };
        })
    .AddJwtBearer(
        JwtBearerDefaults.AuthenticationScheme,
        options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtEmisor,

                ValidateAudience = true,
                ValidAudience = jwtAudiencia,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = claveFirma,

                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(30),

                NameClaimType = ClaimTypes.Name,
                RoleClaimType = ClaimTypes.Role
            };
        });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllers();

// el JavaScript del chat se conecta a esta dirección para hablar con el hub
app.MapHub<ChatIntercambioHub>("/chatIntercambioHub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();