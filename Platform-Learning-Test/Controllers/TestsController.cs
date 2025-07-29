using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platform_Learning_Test.Domain.Dto;
using Platform_Learning_Test.Domain.Entities;
using Platform_Learning_Test.Domain.Extensions;
using Platform_Learning_Test.Models;
using Platform_Learning_Test.Service.Service;
using Platform_Learning_Test.Common.Profiles;
using AutoMapper;
using Platform_Learning_Test.Data.Context;

namespace Platform_Learning_Test.Controllers
{
    [Authorize]
    [Route("Tests")]
    public class TestsController : Controller
    {
        private readonly ITestService _testService;
        private readonly ILogger<TestsController> _logger;
        private readonly IMapper _mapper;
        private readonly ApplicationContext _context;

        public TestsController(
            ITestService testService,
            ILogger<TestsController> logger, IMapper mapper)
        {
            _testService = testService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("")]
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
        public async Task<IActionResult> Create(CreateTestDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            try
            {
                await _testService.CreateTestAsync(new Test
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    Difficulty = Enum.Parse<TestDifficulty>(dto.Difficulty),
                    Category = dto.Category,
                    CreatedAt = DateTime.UtcNow,
                    Duration = TimeSpan.FromHours(1),
                    IsFeatured = false
                });

                return RedirectToAction("Index", "AdminDashboard");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating test");
                ModelState.AddModelError("", $"Ошибка: {ex.Message}");
                return View(dto);
            }
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
            return RedirectToAction("Index", "AdminDashboard");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _testService.DeleteTestAsync(id);
                return RedirectToAction("Index", "AdminDashboard");
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
                return RedirectToAction("Index", "AdminDashboard");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting test with id {id}");
                return View("Error");
            }
        }
        [HttpPost("SubmitTest")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitTest(SubmitTestDto submission)
        {
            submission.EndTime = DateTime.UtcNow;

            var duration = submission.EndTime - submission.StartTime;

            var test = await _testService.GetTestAsync(submission.TestId);

            if (duration.TotalSeconds > test.TimeLimitSeconds)
            {
                return BadRequest("Test time exceeded");
            }

            var userId = User.GetUserId(); 

            var result = await _testService.SaveTestResultsAsync(
                submission.TestId,
                userId,
                submission.Answers);

            return RedirectToAction("Results", new { testId = submission.TestId, resultId = result.Id });
        }

        public async Task<IActionResult> Results(int resultId)
        {
            var result = await _testService.GetTestResultAsync(resultId);
            return View(result);
        }
    }
}
