namespace FoodDispatchSystem.Web.ViewModels
{
    public class ProductSalesViewModel
    {
        public string ProductName { get; set; } = string.Empty;

        public int QuantitySold { get; set; }

        public decimal TotalSales { get; set; }

        public List<ProductSalesViewModel> TopProducts { get; set; }
    = new List<ProductSalesViewModel>();
    }
}