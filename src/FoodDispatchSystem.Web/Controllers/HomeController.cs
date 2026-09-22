using FoodDispatchSystem.Web.Data;
using FoodDispatchSystem.Web.Models;
using FoodDispatchSystem.Web.Services;
using FoodDispatchSystem.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;


namespace FoodDispatchSystem.Web.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly BusinessTimeService _businessTimeService;

    
    public HomeController(
        ILogger<HomeController> logger,
        ApplicationDbContext context,
        BusinessTimeService businessTimeService)
    {
        _logger = logger;
        _context = context;
        _businessTimeService = businessTimeService;
    }
    public async Task<IActionResult> Index()
    {
        var localToday = _businessTimeService.LocalToday;

        var (todayUtc, tomorrowUtc) =
            _businessTimeService.GetUtcRangeForLocalDate(localToday);

        var model = new DashboardViewModel
        {
            OrdersToday = await _context.Orders
    .CountAsync(o =>
        o.CreatedAt >= todayUtc &&
        o.CreatedAt < tomorrowUtc),

            PendingOrders = await _context.Orders
                .CountAsync(o =>
                    o.Status == OrderStatus.Pending),

            PreparingOrders = await _context.Orders
                .CountAsync(o =>
                    o.Status == OrderStatus.Preparing),

            ReadyOrders = await _context.Orders
                .CountAsync(o =>
                    o.Status == OrderStatus.Ready),
            DeliveredToday = await _context.Orders
    .CountAsync(o =>
        o.Status == OrderStatus.Delivered &&
        o.CreatedAt >= todayUtc &&
        o.CreatedAt < tomorrowUtc),

            DeliveredTotalToday = await _context.Orders
    .Where(o =>
        o.Status == OrderStatus.Delivered &&
        o.CreatedAt >= todayUtc &&
        o.CreatedAt < tomorrowUtc)
    .SumAsync(o => (decimal?)o.Total) ?? 0,
               


                RecentOrders = await _context.Orders
    .Include(o => o.OrderDetails)
    .OrderByDescending(o => o.CreatedAt)
    .Take(5)
    .ToListAsync()
        };

        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(
        Duration = 0,
        Location = ResponseCacheLocation.None,
        NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id
                ?? HttpContext.TraceIdentifier
        });
    }
}