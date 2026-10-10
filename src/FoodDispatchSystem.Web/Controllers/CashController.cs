using FoodDispatchSystem.Web.Data;
using FoodDispatchSystem.Web.Models;
using FoodDispatchSystem.Web.ViewsModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodDispatchSystem.Web.Services;

namespace FoodDispatchSystem.Web.Controllers
{
    [Authorize(Roles = "Administrador,Cajero")]
    public class CashController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly BusinessTimeService _businessTimeService;

        public CashController(
      ApplicationDbContext context,
      BusinessTimeService businessTimeService)
        {
            _context = context;
            _businessTimeService = businessTimeService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var localToday = _businessTimeService.LocalToday;

            var (startUtc, endUtc) =
                _businessTimeService.GetUtcRangeForLocalDate(localToday);
            var payments = await _context.Payments
                .AsNoTracking()
                .Where(p => p.PaidAt >= startUtc &&
                            p.PaidAt < endUtc)
                .ToListAsync();
            var model = new CashSummaryViewModel
            {
                From = localToday,
                To = localToday.AddDays(1),
                TotalCollected = payments.Sum(p => p.Amount),

                CashTotal = payments
                    .Where(p => p.Method == PaymentMethod.Cash)
                    .Sum(p => p.Amount),

                CardTotal = payments
                    .Where(p => p.Method == PaymentMethod.Card)
                    .Sum(p => p.Amount),

                PaymentCount = payments.Count,

                CashierEmail = User.Identity?.Name
            };

            return View(model);
        }
    }
}