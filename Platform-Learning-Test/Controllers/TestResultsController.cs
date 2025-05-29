using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Platform_Learning_Test.Service.Service;

namespace Platform_Learning_Test.Controllers
{
    
    public class TestResultsController : Controller
    {
        private readonly ITestResultService _testResultService;
        private readonly ILogger<TestResultsController> _logger;

        public TestResultsController(
            ITestResultService testResultService,
            ILogger<TestResultsController> logger)
        {
            _testResultService = testResultService;
            _logger = logger;
        }

        [HttpGet("Test/{testId}")]
        public async Task<IActionResult> Index(int testId)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var results = await _testResultService.GetUserTestResultsAsync(testId, userId);
                return View(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting test results for test {testId}");
                return View("Error");
            }
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var result = await _testResultService.GetTestResultDetailsAsync(id);
                if (result == null)
                    return NotFound();

                return View(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting test result details with id {id}");
                return View("Error");
            }
        }
    }

}
