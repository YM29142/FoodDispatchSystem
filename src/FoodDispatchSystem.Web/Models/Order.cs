using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodDispatchSystem.Web.Models;

public class Order
{
    public int Id { get; set; }

    [Required]
    [StringLength(20)]
    public string OrderNumber { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    [Column(TypeName = "decimal(10,2)")]
    public decimal Total { get; set; }

    public ICollection<OrderDetail> OrderDetails { get; set; }
    = new List<OrderDetail>();
}