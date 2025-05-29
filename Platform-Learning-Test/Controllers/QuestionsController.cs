using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platform_Learning_Test.Domain.Dto;
using Platform_Learning_Test.Service.Service;

namespace Platform_Learning_Test.Controllers
{
    [Authorize(Roles = "Admin,Teacher")]
    public class QuestionsController : Controller
    {
        private readonly IQuestionService _questionService;
        private readonly ITestService _testService;
        private readonly ILogger<QuestionsController> _logger;

        public QuestionsController(
            IQuestionService questionService,
            ITestService testService,
            ILogger<QuestionsController> logger)
        {
            _questionService = questionService;
            _testService = testService;
            _logger = logger;
        }

        [HttpGet("Test/{testId}")]
        public async Task<IActionResult> Index(int testId)
        {
            try
            {
                var test = await _testService.GetTestWithDetailsAsync(testId);
                if (test == null)
                    return NotFound();

                var questions = await _questionService.GetQuestionsForTestAsync(testId);
                ViewBag.TestId = testId;
                return View(questions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting questions for test {testId}");
                return View("Error");
            }
        }

        [HttpGet("Create/{testId}")]
        public async Task<IActionResult> Create(int testId)
        {
            try
            {
                var test = await _testService.GetTestWithDetailsAsync(testId);
                if (test == null)
                    return NotFound();

                ViewBag.TestId = testId;
                return View(new CreateQuestionDto { TestId = testId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting create question view for test {testId}");
                return View("Error");
            }
        }

        [HttpPost("Create/{testId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int testId, CreateQuestionDto questionDto)
        {
            if (testId != questionDto.TestId)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                ViewBag.TestId = testId;
                return View(questionDto);
            }

            try
            {
                await _questionService.CreateQuestionAsync(questionDto);
                return RedirectToAction(nameof(Index), new { testId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating question for test {testId}");
                ModelState.AddModelError("", "Error creating question");
                ViewBag.TestId = testId;
                return View(questionDto);
            }
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var question = await _questionService.GetQuestionAsync(id);
                if (question == null)
                    return NotFound();

                var updateDto = new UpdateQuestionDto
                {
                    Id = question.Id,
                    Text = question.Text,
                    Difficulty = question.Difficulty.ToString(),
                    TimeLimitSeconds = question.TimeLimitSeconds,
                    TestId = question.TestId
                };

                return View(updateDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting question for edit with id {id}");
                return View("Error");
            }
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateQuestionDto questionDto)
        {
            if (id != questionDto.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(questionDto);

            try
            {
                await _questionService.UpdateQuestionAsync(questionDto);
                return RedirectToAction(nameof(Index), new { testId = questionDto.TestId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating question with id {id}");
                ModelState.AddModelError("", "Error updating question");
                return View(questionDto);
            }
        }

        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var question = await _questionService.GetQuestionAsync(id);
                if (question == null)
                    return NotFound();

                return View(question);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting question for delete with id {id}");
                return View("Error");
            }
        }

        [HttpPost("Delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var question = await _questionService.GetQuestionAsync(id);
                if (question == null)
                    return NotFound();

                var testId = question.TestId;
                await _questionService.DeleteQuestionAsync(id);
                return RedirectToAction(nameof(Index), new { testId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting question with id {id}");
                return View("Error");
            }
        }
    }

}
