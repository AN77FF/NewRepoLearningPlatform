using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platform_Learning_Test.Domain.Dto;
using Platform_Learning_Test.Service.Service;

namespace Platform_Learning_Test.Controllers
{
    [Authorize(Roles = "Admin,Teacher")]
    public class AnswerOptionsController : Controller
    {
        private readonly IAnswerService _answerService;
        private readonly IQuestionService _questionService;
        private readonly ILogger<AnswerOptionsController> _logger;

        public AnswerOptionsController(
            IAnswerService answerService,
            IQuestionService questionService,
            ILogger<AnswerOptionsController> logger)
        {
            _answerService = answerService;
            _questionService = questionService;
            _logger = logger;
        }

        [HttpGet("Question/{questionId}")]
        public async Task<IActionResult> Index(int questionId)
        {
            try
            {
                var question = await _questionService.GetQuestionAsync(questionId);
                if (question == null)
                    return NotFound();

                var answers = await _answerService.GetAnswersForQuestionAsync(questionId);
                ViewBag.QuestionId = questionId;
                return View(answers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting answers for question {questionId}");
                return View("Error");
            }
        }

        [HttpGet("Create/{questionId}")]
        public async Task<IActionResult> Create(int questionId)
        {
            try
            {
                var question = await _questionService.GetQuestionAsync(questionId);
                if (question == null)
                    return NotFound();

                ViewBag.QuestionId = questionId;
                return View(new CreateAnswerOptionDto { QuestionId = questionId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting create answer view for question {questionId}");
                return View("Error");
            }
        }

        [HttpPost("Create/{questionId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int questionId, CreateAnswerOptionDto answerDto)
        {
            if (questionId != answerDto.QuestionId)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                ViewBag.QuestionId = questionId;
                return View(answerDto);
            }

            try
            {
                await _answerService.CreateAnswerAsync(answerDto);
                return RedirectToAction(nameof(Index), new { questionId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating answer for question {questionId}");
                ModelState.AddModelError("", "Error creating answer");
                ViewBag.QuestionId = questionId;
                return View(answerDto);
            }
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var answer = await _answerService.GetAnswerAsync(id);
                if (answer == null)
                    return NotFound();

                var updateDto = new UpdateAnswerOptionDto
                {
                    Id = answer.Id,
                    Text = answer.Text,
                    IsCorrect = answer.IsCorrect,
                    QuestionId = answer.QuestionId
                };

                return View(updateDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting answer for edit with id {id}");
                return View("Error");
            }
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateAnswerOptionDto answerDto)
        {
            if (id != answerDto.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(answerDto);

            try
            {
                await _answerService.UpdateAnswerAsync(answerDto);
                return RedirectToAction(nameof(Index), new { questionId = answerDto.QuestionId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating answer with id {id}");
                ModelState.AddModelError("", "Error updating answer");
                return View(answerDto);
            }
        }

        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var answer = await _answerService.GetAnswerAsync(id);
                if (answer == null)
                    return NotFound();

                return View(answer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting answer for delete with id {id}");
                return View("Error");
            }
        }

        [HttpPost("Delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var answer = await _answerService.GetAnswerAsync(id);
                if (answer == null)
                    return NotFound();

                var questionId = answer.QuestionId;
                await _answerService.DeleteAnswerAsync(id);
                return RedirectToAction(nameof(Index), new { questionId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting answer with id {id}");
                return View("Error");
            }
        }
    }

}
