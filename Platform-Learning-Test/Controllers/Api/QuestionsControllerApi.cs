using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platform_Learning_Test.Domain.Dto;
using Platform_Learning_Test.Service.Service;

namespace Platform_Learning_Test.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionsControllerApi : ControllerBase
    {
        private readonly IQuestionService _questionService;
        private readonly ILogger<QuestionsControllerApi> _logger;

        public QuestionsControllerApi(
            IQuestionService questionService,
            ILogger<QuestionsControllerApi> logger)
        {
            _questionService = questionService;
            _logger = logger;
        }

        [HttpGet("Test/{testId}")]
        public async Task<ActionResult<IEnumerable<QuestionDto>>> GetQuestionsForTest(int testId)
        {
            try
            {
                var questions = await _questionService.GetQuestionsForTestAsync(testId);
                return Ok(questions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting questions for test {testId}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<QuestionDto>> GetQuestion(int id)
        {
            try
            {
                var question = await _questionService.GetQuestionAsync(id);
                if (question == null)
                    return NotFound();

                return Ok(question);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting question with id {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<ActionResult<QuestionDto>> CreateQuestion(CreateQuestionDto questionDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var question = await _questionService.CreateQuestionAsync(questionDto);
                return CreatedAtAction(nameof(GetQuestion), new { id = question.Id }, question);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating question");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> UpdateQuestion(int id, UpdateQuestionDto questionDto)
        {
            try
            {
                if (id != questionDto.Id)
                    return BadRequest("ID mismatch");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _questionService.UpdateQuestionAsync(questionDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating question with id {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            try
            {
                await _questionService.DeleteQuestionAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting question with id {id}");
                return StatusCode(500, "Internal server error");
            }
        }
    }

}
