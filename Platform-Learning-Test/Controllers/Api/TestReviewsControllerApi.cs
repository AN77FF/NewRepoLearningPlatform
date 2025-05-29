using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Platform_Learning_Test.Service.Service;
using Platform_Learning_Test.Domain.Dto;

namespace Platform_Learning_Test.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TestReviewsControllerApi : ControllerBase
    {
        private readonly ITestReviewService _reviewService;
        private readonly ILogger<TestReviewsControllerApi> _logger;

        public TestReviewsControllerApi(
            ITestReviewService reviewService,
            ILogger<TestReviewsControllerApi> logger)
        {
            _reviewService = reviewService;
            _logger = logger;
        }

        [HttpGet("Test/{testId}")]
        public async Task<ActionResult<IEnumerable<TestReviewDto>>> GetReviews(int testId)
        {
            try
            {
                var reviews = await _reviewService.GetReviewsForTestAsync(testId);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting reviews for test {testId}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<TestReviewDto>> CreateReview(CreateTestReviewDto reviewDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var review = await _reviewService.CreateReviewAsync(reviewDto, userId);
                return CreatedAtAction(nameof(GetReview), new { id = review.Id }, review);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating review");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TestReviewDto>> GetReview(int id)
        {
            try
            {
                var review = await _reviewService.GetReviewAsync(id);
                if (review == null) return NotFound();
                return Ok(review);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting review with id {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            try
            {
                await _reviewService.DeleteReviewAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting review with id {id}");
                return StatusCode(500, "Internal server error");
            }
        }
    }

}
