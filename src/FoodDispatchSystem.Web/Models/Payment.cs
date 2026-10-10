using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodDispatchSystem.Web.Models
{
    public class Payment
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; } = null!;

        public PaymentMethod Method { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        [Required]
        public string ReceivedByUserId { get; set; } = string.Empty;

        [Required]
        [MaxLength(256)]
        public string ReceivedByEmail { get; set; } = string.Empty;

        public DateTime PaidAt { get; set; } = DateTime.UtcNow;
    }
}