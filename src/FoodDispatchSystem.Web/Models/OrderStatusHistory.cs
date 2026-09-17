namespace FoodDispatchSystem.Web.Models
{
    public class OrderStatusHistory
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public OrderStatus PreviousStatus { get; set; }

        public OrderStatus NewStatus { get; set; }

        public DateTime ChangedAt { get; set; }

        public string ChangedByUserId { get; set; } = string.Empty;

        public string ChangedByEmail { get; set; } = string.Empty;

        public Order Order { get; set; } = null!;
    }
}