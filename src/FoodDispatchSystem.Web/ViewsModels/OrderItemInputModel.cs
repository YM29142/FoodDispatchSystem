using System.ComponentModel.DataAnnotations;

namespace FoodDispatchSystem.Web.ViewModels;

public class OrderItemInputModel
{
    [Required(ErrorMessage = "Debe seleccionar un producto.")]
    [Range(1, int.MaxValue, ErrorMessage = "El producto seleccionado no es válido.")]
    public int? ProductId { get; set; }

    [Range(1, 100, ErrorMessage = "La cantidad debe estar entre 1 y 100.")]
    public int Quantity { get; set; }
}