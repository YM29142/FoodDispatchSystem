using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodDispatchSystem.Web.Models;

public class Product
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(250)]
    public string? Description { get; set; }

    [Required]
    [Range(0.01, 99999.99, ErrorMessage = "El precio debe ser mayor que 0.")]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }

    public bool IsActive { get; set; } = true;

    public int CategoryId { get; set; }

    [ValidateNever]
    public Category Category { get; set; } = null!;
}