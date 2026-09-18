namespace FoodDispatchSystem.Web.ViewModels
{
    public class ReportsViewModel
    {
        public int TotalOrders { get; set; }

        public int DeliveredOrders { get; set; }

        public int CancelledOrders { get; set; }

        public decimal TotalSales { get; set; }

        public decimal AverageTicket { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public List<ProductSalesViewModel> TopProducts { get; set; }
    = new List<ProductSalesViewModel>();

        public List<EmployeeSalesViewModel> EmployeeSales { get; set; }
    = new List<EmployeeSalesViewModel>();

        public List<DailySalesViewModel> DailySales { get; set; }
    = new List<DailySalesViewModel>();
    }
}