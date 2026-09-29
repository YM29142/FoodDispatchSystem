using System.ComponentModel.DataAnnotations;
using FoodDispatchSystem.Web.Models;

namespace FoodDispatchSystem.Web.ViewModels;

public class OrderCreateViewModel
{
    public OrderType OrderType { get; set; } = OrderType.DineIn;
    public int? TableNumber { get; set; }

    [MinLength(1, ErrorMessage = "Debe agregar al menos un producto.")]
    public List<OrderItemInputModel> Items { get; set; }
        = new List<OrderItemInputModel>();
}