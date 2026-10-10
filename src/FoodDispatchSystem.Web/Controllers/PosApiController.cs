using FoodDispatchSystem.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodDispatchSystem.Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/pos")]
    public class PosApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PosApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _context.Products
                .AsNoTracking()
                .Where(p => p.IsActive && p.Category.IsActive)
                .OrderBy(p => p.Name)
                .Select(p => new
                {
                    id = p.Id,
                    name = p.Name,
                    price = p.Price,
                    categoryId = p.CategoryId,
                    category = p.Category.Name
                })
                .ToListAsync();

            return Ok(products);
        }
    }
}