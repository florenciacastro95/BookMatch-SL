using System.ComponentModel.DataAnnotations;
using Bookmatch_SL.Models.Enums;

namespace Bookmatch_SL.ViewModels;

public class UsuarioAdminEditarViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es obligatorio.")]
    [EmailAddress(ErrorMessage = "Ingresá un email válido.")]
    [StringLength(254)]
    public string Email { get; set; } = string.Empty;

    public Rol Rol { get; set; }

    public bool Activo { get; set; }
}