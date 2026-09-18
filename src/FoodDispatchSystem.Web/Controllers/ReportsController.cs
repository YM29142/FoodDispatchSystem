using FoodDispatchSystem.Web.Data;
using FoodDispatchSystem.Web.Models;
using FoodDispatchSystem.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodDispatchSystem.Web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index(
            DateTime? startDate,
            DateTime? endDate)
        {
            if (startDate.HasValue &&
    endDate.HasValue &&
    startDate.Value.Date > endDate.Value.Date)
            {
                ViewBag.DateError =
                    "La fecha inicial no puede ser posterior a la fecha final.";

                return View(new ReportsViewModel
                {
                    StartDate = startDate,
                    EndDate = endDate
                });
            }
            var ordersQuery = _context.Orders
                .AsNoTracking()
                .AsQueryable();

            if (startDate.HasValue)
            {
                var start = startDate.Value.Date;

                ordersQuery = ordersQuery
                    .Where(o => o.CreatedAt >= start);
            }

            if (endDate.HasValue)
            {
                var endExclusive =
                    endDate.Value.Date.AddDays(1);

                ordersQuery = ordersQuery
                    .Where(o => o.CreatedAt < endExclusive);
            }

            var totalOrders = await ordersQuery
                .CountAsync();

            var deliveredOrders = await ordersQuery
                .CountAsync(o =>
                    o.Status == OrderStatus.Delivered);

            var cancelledOrders = await ordersQuery
                .CountAsync(o =>
                    o.Status == OrderStatus.Cancelled);

            var totalSales = await ordersQuery
                .Where(o =>
                    o.Status == OrderStatus.Delivered)
                .SumAsync(o => (decimal?)o.Total)
                ?? 0m;

            var averageTicket =
                deliveredOrders > 0
                    ? totalSales / deliveredOrders
                    : 0m;

            var deliveredOrderIds = ordersQuery
    .Where(o => o.Status == OrderStatus.Delivered)
    .Select(o => o.Id);

            var topProducts = await _context.OrderDetails
                .AsNoTracking()
                .Where(d => deliveredOrderIds.Contains(d.OrderId))
                .GroupBy(d => new
                {
                    d.ProductId,
                    ProductName = d.Product.Name
                })
                .Select(g => new ProductSalesViewModel
                {
                    ProductName = g.Key.ProductName,
                    QuantitySold = g.Sum(d => d.Quantity),
                    TotalSales = g.Sum(d => d.Subtotal)
                })
                .OrderByDescending(p => p.QuantitySold)
                .ThenByDescending(p => p.TotalSales)
                .Take(5)
                .ToListAsync();
            var employeeSales = await ordersQuery
    .Where(o => o.Status == OrderStatus.Delivered)
    .GroupBy(o => o.CreatedByEmail ?? "No disponible")
    .Select(g => new EmployeeSalesViewModel
    {
        EmployeeEmail = g.Key,
        OrdersCount = g.Count(),
        TotalSales = g.Sum(o => o.Total)
    })
    .OrderByDescending(e => e.TotalSales)
    .ToListAsync();

            var dailySales = await ordersQuery
    .Where(o => o.Status == OrderStatus.Delivered)
    .GroupBy(o => o.CreatedAt.Date)
    .Select(g => new DailySalesViewModel
    {
        Date = g.Key,
        OrdersCount = g.Count(),
        TotalSales = g.Sum(o => o.Total)
    })
    .OrderBy(d => d.Date)
    .ToListAsync();

            var model = new ReportsViewModel
            {
                TotalOrders = totalOrders,
                DeliveredOrders = deliveredOrders,
                CancelledOrders = cancelledOrders,
                TotalSales = totalSales,
                AverageTicket = averageTicket,
                StartDate = startDate,
                EndDate = endDate,
                TopProducts = topProducts,
                EmployeeSales = employeeSales,
                DailySales = dailySales
            };

            return View(model);
        }
    }
}