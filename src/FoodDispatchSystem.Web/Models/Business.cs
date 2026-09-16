using System.ComponentModel.DataAnnotations;

namespace FoodDispatchSystem.Web.Models;

public class Business
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [StringLength(250)]
    public string? Address { get; set; }

    [StringLength(30)]
    public string? Phone { get; set; }

    [EmailAddress]
    [StringLength(150)]
    public string? Email { get; set; }

    [StringLength(300)]
    public string? LogoPath { get; set; }

    [Required]
    [StringLength(3)]
    public string CurrencyCode { get; set; } = "USD";

    public bool IsConfigured { get; set; } = false;
}