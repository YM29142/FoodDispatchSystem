using System.ComponentModel.DataAnnotations;

namespace FoodDispatchSystem.Web.ViewModels;

public class BusinessSetupViewModel
{
    [Required(ErrorMessage = "El nombre del negocio es obligatorio.")]
    [StringLength(150)]
    [Display(Name = "Nombre del negocio")]
    public string Name { get; set; } = string.Empty;

    [StringLength(250)]
    [Display(Name = "Dirección")]
    public string? Address { get; set; }

    [StringLength(30)]
    [Display(Name = "Teléfono")]
    public string? Phone { get; set; }

    [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido.")]
    [StringLength(150)]
    [Display(Name = "Correo electrónico")]
    public string? Email { get; set; }

    [Required]
    [StringLength(3)]
    [Display(Name = "Moneda")]
    public string CurrencyCode { get; set; } = "USD";

    [Required(ErrorMessage = "El correo del administrador es obligatorio.")]
    [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido.")]
    [Display(Name = "Correo del administrador")]
    public string AdminEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña del administrador es obligatoria.")]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
    [Display(Name = "Contraseña")]
    public string AdminPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirme la contraseña.")]
    [DataType(DataType.Password)]
    [Compare(
        nameof(AdminPassword),
        ErrorMessage = "Las contraseñas no coinciden.")]
    [Display(Name = "Confirmar contraseña")]
    public string ConfirmPassword { get; set; } = string.Empty;
}