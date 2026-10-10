using System.ComponentModel.DataAnnotations;
using FoodDispatchSystem.Web.Models;

namespace FoodDispatchSystem.Web.ViewModels
{
    public class OrderPaymentViewModel
    {
        public int OrderId { get; set; }

        public string OrderNumber { get; set; } = string.Empty;

        public decimal OrderTotal { get; set; }

        public decimal PaidAmount { get; set; }

        public decimal RemainingAmount { get; set; }

        [Required(ErrorMessage = "Seleccione un método de pago.")]
        public PaymentMethod Method { get; set; }

        [Range(0.01, 999999.99, ErrorMessage = "Ingrese un monto válido.")]
        public decimal Amount { get; set; }
    }
}