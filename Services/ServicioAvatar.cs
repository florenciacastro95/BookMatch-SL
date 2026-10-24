namespace Bookmatch_SL.Services;

public class ServicioAvatar
{
    private readonly IWebHostEnvironment entorno;

    private const long TamanoMaximo = 2 * 1024 * 1024;

    private static readonly Dictionary<string, string> TiposPermitidos =
        new(StringComparer.OrdinalIgnoreCase)
        {
            [".jpg"] = "image/jpeg",
            [".jpeg"] = "image/jpeg",
            [".png"] = "image/png",
            [".webp"] = "image/webp"
        };

    public ServicioAvatar(IWebHostEnvironment entorno)
    {
        this.entorno = entorno;
    }

    public async Task<string?> GuardarAsync(IFormFile? archivo)
    {
        if (archivo == null || archivo.Length == 0)
            return null;

        if (archivo.Length > TamanoMaximo)
        {
            throw new InvalidDataException(
                "El avatar no puede superar los 2 MB.");
        }

        string extension = Path.GetExtension(archivo.FileName);

        if (!TiposPermitidos.TryGetValue(extension, out string? tipoEsperado))
        {
            throw new InvalidDataException(
                "El avatar debe ser JPG, PNG o WEBP.");
        }

        if (!string.Equals(
                archivo.ContentType,
                tipoEsperado,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidDataException(
                "El tipo de archivo no coincide con la imagen.");
        }

        string carpetaWeb = entorno.WebRootPath
            ?? Path.Combine(entorno.ContentRootPath, "wwwroot");

        string carpetaAvatares = Path.Combine(
            carpetaWeb,
            "uploads",
            "avatares");

        Directory.CreateDirectory(carpetaAvatares);

        string nombreArchivo = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";

        string rutaCompleta = Path.Combine(
            carpetaAvatares,
            nombreArchivo);

        await using (var stream = new FileStream(
            rutaCompleta,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None))
        {
            await archivo.CopyToAsync(stream);
        }

        return $"/uploads/avatares/{nombreArchivo}";
    }

    public void EliminarSiExiste(string? urlAvatar)
    {
        if (string.IsNullOrWhiteSpace(urlAvatar))
            return;

        const string prefijo = "/uploads/avatares/";

        if (!urlAvatar.StartsWith(
                prefijo,
                StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        string nombreArchivo = Path.GetFileName(urlAvatar);

        string carpetaWeb = entorno.WebRootPath
            ?? Path.Combine(entorno.ContentRootPath, "wwwroot");

        string rutaCompleta = Path.Combine(
            carpetaWeb,
            "uploads",
            "avatares",
            nombreArchivo);

        if (File.Exists(rutaCompleta))
            File.Delete(rutaCompleta);
    }
}