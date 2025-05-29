using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platform_Learning_Test.Domain.Dto;
using Platform_Learning_Test.Service.Service;

namespace Platform_Learning_Test.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestsControllerApi : ControllerBase
    {
        private readonly ITestService _testService;
        private readonly ILogger<TestsControllerApi> _logger;

        public TestsControllerApi(
            ITestService testService,
            ILogger<TestsControllerApi> logger)
        {
            _testService = testService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TestDto>>> GetTests()
        {
            try
            {
                var tests = await _testService.GetAllTestsAsync();
                return Ok(tests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tests");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TestDetailDto>> GetTest(int id)
        {
            try
            {
                var test = await _testService.GetTestWithDetailsAsync(id);
                if (test == null)
                    return NotFound();

                return Ok(test);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting test with id {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<ActionResult<TestDto>> CreateTest(CreateTestDto testDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var test = await _testService.CreateTestAsync(testDto);
                return CreatedAtAction(nameof(GetTest), new { id = test.Id }, test);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating test");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> UpdateTest(int id, UpdateTestDto testDto)
        {
            try
            {
                if (id != testDto.Id)
                    return BadRequest("ID mismatch");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _testService.UpdateTestAsync(testDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating test with id {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTest(int id)
        {
            try
            {
                await _testService.DeleteTestAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting test with id {id}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
