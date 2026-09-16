using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace FoodDispatchSystem.Web.ViewModels;

public class BusinessEditViewModel
{
    public int Id { get; set; }

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

    [Display(Name = "Logo del negocio")]
    public IFormFile? LogoFile { get; set; }

    public string? CurrentLogoPath { get; set; }
}
