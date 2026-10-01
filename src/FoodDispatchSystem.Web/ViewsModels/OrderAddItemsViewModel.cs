using System.ComponentModel.DataAnnotations;

namespace FoodDispatchSystem.Web.ViewModels
{
    public class OrderAddItemsViewModel
    {
        public int OrderId { get; set; }

        public string OrderNumber { get; set; }
            = string.Empty;

        public int? TableNumber { get; set; }

        [MinLength(
            1,
            ErrorMessage = "Debe agregar al menos un producto.")]
        public List<OrderItemInputModel> Items { get; set; }
            = new();
    }
}