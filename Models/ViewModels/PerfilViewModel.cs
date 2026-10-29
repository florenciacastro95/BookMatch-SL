using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Bookmatch_SL.ViewModels;

public class PerfilViewModel
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es obligatorio.")]
    [EmailAddress(ErrorMessage = "Ingresá un email válido.")]
    [StringLength(254)]
    public string Email { get; set; } = string.Empty;

    public string? AvatarActualUrl { get; set; }

    public IFormFile? Avatar { get; set; }
}