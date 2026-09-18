namespace FoodDispatchSystem.Web.ViewModels
{
    public class DailySalesViewModel
    {
        public DateTime Date { get; set; }

        public int OrdersCount { get; set; }

        public decimal TotalSales { get; set; }
    }
}