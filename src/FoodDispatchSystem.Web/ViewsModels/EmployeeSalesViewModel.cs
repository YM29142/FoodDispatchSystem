namespace FoodDispatchSystem.Web.ViewModels
{
    public class EmployeeSalesViewModel
    {
        public string EmployeeEmail { get; set; } = string.Empty;

        public int OrdersCount { get; set; }

        public decimal TotalSales { get; set; }
    }
}