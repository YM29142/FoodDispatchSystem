using FoodDispatchSystem.Web.Data;
using FoodDispatchSystem.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodDispatchSystem.Web.Services
{
    public class BusinessService
    {
        private readonly ApplicationDbContext _context;

        public BusinessService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Business?> GetBusinessAsync()
        {
            return await _context.Businesses
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.IsConfigured);
        }
    }
}