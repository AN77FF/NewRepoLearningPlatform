using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platform_Learning_Test.Domain.Dto;
using Platform_Learning_Test.Service.Service;

namespace Platform_Learning_Test.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Teacher")]
    public class AnswerOptionsControllerApi : ControllerBase
    {
        private readonly IAnswerService _answerService;
        private readonly ILogger<AnswerOptionsControllerApi> _logger;

        public AnswerOptionsControllerApi(
            IAnswerService answerService,
            ILogger<AnswerOptionsControllerApi> logger)
        {
            _answerService = answerService;
            _logger = logger;
        }

        [HttpGet("Question/{questionId}")]
        public async Task<ActionResult<IEnumerable<AnswerOptionDto>>> GetAnswersForQuestion(int questionId)
        {
            try
            {
                var answers = await _answerService.GetAnswersForQuestionAsync(questionId);
                return Ok(answers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting answers for question {questionId}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AnswerOptionDto>> GetAnswer(int id)
        {
            try
            {
                var answer = await _answerService.GetAnswerAsync(id);
                if (answer == null)
                    return NotFound();

                return Ok(answer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting answer with id {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<AnswerOptionDto>> CreateAnswer(CreateAnswerOptionDto answerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var answer = await _answerService.CreateAnswerAsync(answerDto);
                return CreatedAtAction(nameof(GetAnswer), new { id = answer.Id }, answer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating answer");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAnswer(int id, UpdateAnswerOptionDto answerDto)
        {
            try
            {
                if (id != answerDto.Id)
                    return BadRequest("ID mismatch");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _answerService.UpdateAnswerAsync(answerDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating answer with id {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnswer(int id)
        {
            try
            {
                await _answerService.DeleteAnswerAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting answer with id {id}");
                return StatusCode(500, "Internal server error");
            }
        }
    }

}
