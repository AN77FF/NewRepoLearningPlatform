using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platform_Learning_Test.Domain.Dto;
using Platform_Learning_Test.Models;
using Platform_Learning_Test.Service.Service;

namespace Platform_Learning_Test.Controllers
{
    [Authorize]
    public class TestsController : Controller
    {
        private readonly ITestService _testService;
        private readonly ILogger<TestsController> _logger;

        public TestsController(
            ITestService testService,
            ILogger<TestsController> logger)
        {
            _testService = testService;
            _logger = logger;
        }


        public async Task<IActionResult> Index(string category = null)
        {
            var tests = await _testService.GetAllTestsAsync();
            var categories = tests.Select(t => t.Category).Distinct().OrderBy(c => c);

            return View(new TestCategoryViewModel
            {
                Categories = categories.ToList(),
                Tests = category == null ? tests : tests.Where(t => t.Category == category)
            });
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTestDto testDto)
        {
            if (!ModelState.IsValid) return View(testDto);

            await _testService.CreateTestAsync(testDto);
            return RedirectToAction(nameof(Index));
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var test = await _testService.GetTestWithDetailsAsync(id);
                return View(test);
            }
            catch
            {
                return NotFound();
            }
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var test = await _testService.GetTestWithDetailsAsync(id);
                return View(new UpdateTestDto
                {
                    Id = test.Id,
                    Title = test.Title,
                    Description = test.Description,
                    Difficulty = test.Difficulty,
                    Category = test.Category
                });
            }
            catch
            {
                return NotFound();
            }
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateTestDto testDto)
        {
            if (id != testDto.Id) return NotFound();

            if (!ModelState.IsValid) return View(testDto);

            await _testService.UpdateTestAsync(testDto);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _testService.DeleteTestAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View("Error");
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _testService.DeleteTestAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting test with id {id}");
                return View("Error");
            }
        }
    }
}
