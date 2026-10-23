using System.ComponentModel.DataAnnotations;

namespace Bookmatch_SL.ViewModels;

public class CambiarPasswordViewModel
{
    [Required(ErrorMessage = "Ingresá tu contraseña actual.")]
    [DataType(DataType.Password)]
    public string PasswordActual { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingresá la nueva contraseña.")]
    [StringLength(100, MinimumLength = 8,
        ErrorMessage = "La nueva contraseña debe tener al menos 8 caracteres.")]
    [DataType(DataType.Password)]
    public string PasswordNueva { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirmá la nueva contraseña.")]
    [Compare(nameof(PasswordNueva),
        ErrorMessage = "Las contraseñas no coinciden.")]
    [DataType(DataType.Password)]
    public string ConfirmarPassword { get; set; } = string.Empty;
}