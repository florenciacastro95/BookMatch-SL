namespace Bookmatch_SL.ViewModels;

public class UsuarioAdminIndexViewModel
{
    public IList<UsuarioAdminItemViewModel> Usuarios { get; set; }
        = new List<UsuarioAdminItemViewModel>();

    public int PaginaActual { get; set; }

    public int TotalPaginas { get; set; }
}

public class UsuarioAdminItemViewModel
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Rol { get; set; } = string.Empty;

    public bool Activo { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? AvatarUrl { get; set; }
}