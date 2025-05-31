using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Platform_Learning_Test.Models;
using Platform_Learning_Test.Service.Service;

namespace Platform_Learning_Test.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITestService _testService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ITestService testService, 
                             ILogger<HomeController> logger)
        {
            _testService = testService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            return View();
            //var featuredTests = await _testService.GetFeaturedTestsAsync();
            //return View(featuredTests);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
