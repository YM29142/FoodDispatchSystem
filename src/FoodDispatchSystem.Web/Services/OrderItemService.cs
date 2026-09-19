using FoodDispatchSystem.Web.ViewModels;

namespace FoodDispatchSystem.Web.Services
{
    public class OrderItemService
    {
        public List<OrderItemInputModel> ConsolidateItems(
            IEnumerable<OrderItemInputModel> items)
        {
            return items
                .GroupBy(i => i.ProductId)
                .Select(g => new OrderItemInputModel
                {
                    ProductId = g.Key,
                    Quantity = g.Sum(i => i.Quantity)
                })
                .ToList();
        }
        public bool ExceedsMaximumQuantity(
    IEnumerable<OrderItemInputModel> items,
    int maximumQuantity = 100)
        {
            var consolidatedItems = ConsolidateItems(items);

            return consolidatedItems
                .Any(i => i.Quantity > maximumQuantity);
        }
    }
}