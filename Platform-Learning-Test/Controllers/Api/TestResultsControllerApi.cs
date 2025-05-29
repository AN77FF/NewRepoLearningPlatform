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
    public class TestResultsControllerApi : ControllerBase
    {
        private readonly ITestResultService _testResultService;
        private readonly ILogger<TestResultsControllerApi> _logger;

        public TestResultsControllerApi(
            ITestResultService testResultService,
            ILogger<TestResultsControllerApi> logger)
        {
            _testResultService = testResultService;
            _logger = logger;
        }


        [HttpGet("Test/{testId}")]
        public async Task<ActionResult<TestResultDto>> GetUserTestResults(int testId)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var results = await _testResultService.GetUserTestResultsAsync(testId, userId);
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting test results for test {testId}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<TestResultDto>> SubmitTest([FromBody] SubmitTestDto submitDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var result = await _testResultService.SubmitTestAsync(submitDto, userId);
                return CreatedAtAction(nameof(GetTestResult), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting test");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TestResultDetailsDto>> GetTestResult(int id)
        {
            try
            {
                var result = await _testResultService.GetTestResultDetailsAsync(id);
                if (result == null)
                    return NotFound();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting test result with id {id}");
                return StatusCode(500, "Internal server error");
            }
        }
    }

}
