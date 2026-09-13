namespace FoodDispatchSystem.Web.ViewModels;

public class DashboardViewModel
{
    public int OrdersToday { get; set; }

    public int PendingOrders { get; set; }

    public int PreparingOrders { get; set; }

    public int ReadyOrders { get; set; }

    public int DeliveredToday { get; set; }

    public decimal DeliveredTotalToday { get; set; }
}