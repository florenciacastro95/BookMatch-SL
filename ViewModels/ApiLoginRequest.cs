using System.ComponentModel.DataAnnotations;

namespace Bookmatch_SL.ViewModels;

public class ApiLoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}