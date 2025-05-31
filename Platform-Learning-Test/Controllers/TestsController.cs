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

       
        [HttpGet]
        public async Task<IActionResult> Index(string category = null)
        {
            var tests = await _testService.GetAllTestsAsync();

            var categories = tests
                .Select(t => t.Category)
                .Distinct()
                .OrderBy(c => c);

            var vm = new TestCategoryViewModel
            {
                Categories = categories.ToList(),
                Tests = category == null
                    ? tests
                    : tests.Where(t => t.Category == category)
            };

            return View(vm);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Create(CreateTestDto testDto)
        {
            if (!ModelState.IsValid)
                return View(testDto);

            try
            {
                await _testService.CreateTestAsync(testDto);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating test");
                ModelState.AddModelError("", "Error creating test");
                return View(testDto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var test = await _testService.GetTestWithDetailsAsync(id);
                if (test == null)
                    return NotFound();

                return View(test);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting test with id {id}");
                return NotFound();
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var test = await _testService.GetTestWithDetailsAsync(id);
                if (test == null)
                    return NotFound();

                var updateDto = new UpdateTestDto
                {
                    Id = test.Id,
                    Title = test.Title,
                    Description = test.Description,
                    Difficulty = test.Difficulty,
                    Category = test.Category
                };

                return View(updateDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting test for edit with id {id}");
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Edit(int id, UpdateTestDto testDto)
        {
            if (id != testDto.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(testDto);

            try
            {
                await _testService.UpdateTestAsync(testDto);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating test with id {id}");
                ModelState.AddModelError("", "Error updating test");
                return View(testDto);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var test = await _testService.GetTestWithDetailsAsync(id);
                if (test == null)
                    return NotFound();

                return View(test);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting test for delete with id {id}");
                return NotFound();
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
