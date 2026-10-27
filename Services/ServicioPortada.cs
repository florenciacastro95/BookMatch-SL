using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Bookmatch_SL.Services;

public class ServicioPortada
{
    private readonly IWebHostEnvironment entorno;

    private const long TamanoMaximo = 5 * 1024 * 1024;

    public ServicioPortada(IWebHostEnvironment entorno)
    {
        this.entorno = entorno;
    }

    public async Task<string> GuardarAsync(IFormFile archivo)
    {
        if (archivo.Length == 0 || archivo.Length > TamanoMaximo)
        {
            throw new InvalidDataException(
                "La portada debe tener contenido y pesar como máximo 5 MB.");
        }

        string extension = Path.GetExtension(archivo.FileName)
            .ToLowerInvariant();

        if (extension is not (".jpg" or ".jpeg" or ".png" or ".webp"))
        {
            throw new InvalidDataException(
                "La portada debe ser JPG, PNG o WEBP.");
        }

        if (!await FirmaValidaAsync(archivo, extension))
        {
            throw new InvalidDataException(
                "El contenido del archivo no coincide con una imagen válida.");
        }

        string carpetaWeb = entorno.WebRootPath
            ?? Path.Combine(entorno.ContentRootPath, "wwwroot");

        string carpetaPortadas = Path.Combine(
            carpetaWeb, "uploads", "portadas");

        Directory.CreateDirectory(carpetaPortadas);

        string nombre = $"{Guid.NewGuid():N}{extension}";
        string ruta = Path.Combine(carpetaPortadas, nombre);

        await using (var stream = new FileStream(
            ruta,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None))
        {
            await archivo.CopyToAsync(stream);
        }

        return $"/uploads/portadas/{nombre}";
    }

    private static async Task<bool> FirmaValidaAsync(
        IFormFile archivo, string extension)
    {
        byte[] cabecera = new byte[12];

        await using var stream = archivo.OpenReadStream();

        int leidos = await stream.ReadAsync(
            cabecera.AsMemory(0, cabecera.Length));

        return extension switch
        {
            ".jpg" or ".jpeg" =>
                leidos >= 3 &&
                cabecera[0] == 0xFF &&
                cabecera[1] == 0xD8 &&
                cabecera[2] == 0xFF,

            ".png" =>
                leidos >= 8 &&
                cabecera.Take(8).SequenceEqual(new byte[]
                {
                    0x89, 0x50, 0x4E, 0x47,
                    0x0D, 0x0A, 0x1A, 0x0A
                }),

            ".webp" =>
                leidos >= 12 &&
                Encoding.ASCII.GetString(cabecera, 0, 4) == "RIFF" &&
                Encoding.ASCII.GetString(cabecera, 8, 4) == "WEBP",

            _ => false
        };
    }

    public void EliminarSiExiste(string? urlPortada)
    {
        const string prefijo = "/uploads/portadas/";

        if (string.IsNullOrWhiteSpace(urlPortada) ||
            !urlPortada.StartsWith(
                prefijo, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        string nombre = Path.GetFileName(urlPortada);

        string carpetaWeb = entorno.WebRootPath
            ?? Path.Combine(entorno.ContentRootPath, "wwwroot");

        string ruta = Path.Combine(
            carpetaWeb, "uploads", "portadas", nombre);

        if (System.IO.File.Exists(ruta))
        {
            System.IO.File.Delete(ruta);
        }
    }
}