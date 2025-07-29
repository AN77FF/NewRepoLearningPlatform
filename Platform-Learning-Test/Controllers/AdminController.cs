using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platform_Learning_Test.Data.Context;
using Platform_Learning_Test.Domain.Entities;

namespace Platform_Learning_Test.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("admin/stats")]
    public class AdminController : Controller
    {
        private readonly ApplicationContext _context;

        public AdminController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet("stats")]
        public IActionResult Dashboard()
        {
            var stats = new AdminDashboardStats
            {
                TotalUsers = _context.Users.Count(),
                TotalTests = _context.Tests.Count(),
                NewUsers = _context.Users.Count(u => u.CreatedAt > DateTime.UtcNow.AddDays(-7)),
                ActiveTests = _context.Tests.Count(t => t.IsFeatured)
            };

            return View(stats);
        }
    }

}
