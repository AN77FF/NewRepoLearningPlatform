using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Platform_Learning_Test.Service.Service;
using Platform_Learning_Test.Domain.Dto;

namespace Platform_Learning_Test.Controllers
{
    [Authorize]
    public class TestReviewsController : Controller
    {
        private readonly ITestReviewService _reviewService;
        private readonly ILogger<TestReviewsController> _logger;

        public TestReviewsController(
            ITestReviewService reviewService,
            ILogger<TestReviewsController> logger)
        {
            _reviewService = reviewService;
            _logger = logger;
        }

        [HttpGet("Test/{testId}")]
        public async Task<IActionResult> Index(int testId)
        {
            try
            {
                var reviews = await _reviewService.GetReviewsForTestAsync(testId);
                ViewBag.TestId = testId;
                return View(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting reviews for test {testId}");
                return View("Error");
            }
        }

        [HttpGet("Create/{testId}")]
        public IActionResult Create(int testId)
        {
            ViewBag.TestId = testId;
            return View(new CreateTestReviewDto { TestId = testId });
        }

        [HttpPost("Create/{testId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int testId, CreateTestReviewDto reviewDto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.TestId = testId;
                return View(reviewDto);
            }

            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                await _reviewService.CreateReviewAsync(reviewDto, userId);
                return RedirectToAction("Index", new { testId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating review for test {testId}");
                ModelState.AddModelError("", "Error creating review");
                ViewBag.TestId = testId;
                return View(reviewDto);
            }
        }

        [HttpPost("Delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var review = await _reviewService.GetReviewAsync(id);
                if (review == null) return NotFound();

                var testId = review.TestId;
                await _reviewService.DeleteReviewAsync(id);
                return RedirectToAction("Index", new { testId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting review with id {id}");
                return View("Error");
            }
        }
    }

}
