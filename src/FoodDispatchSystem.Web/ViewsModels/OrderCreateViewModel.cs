using System.ComponentModel.DataAnnotations;

namespace FoodDispatchSystem.Web.ViewModels;

public class OrderCreateViewModel
{
    [MinLength(1, ErrorMessage = "Debe agregar al menos un producto.")]
    public List<OrderItemInputModel> Items { get; set; }
        = new List<OrderItemInputModel>();
}