namespace FoodDispatchSystem.Web.ViewsModels
{
    public class CashSummaryViewModel
    {
        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public decimal TotalCollected { get; set; }

        public decimal CashTotal { get; set; }

        public decimal CardTotal { get; set; }

        public int PaymentCount { get; set; }

        public string? CashierEmail { get; set; }
    }
}