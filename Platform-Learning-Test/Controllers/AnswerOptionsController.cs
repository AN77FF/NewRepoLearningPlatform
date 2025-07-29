using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Platform_Learning_Test.Data.Context;
using Platform_Learning_Test.Domain.Dto;
using Platform_Learning_Test.Domain.Entities;
using Platform_Learning_Test.Service.Service;
using SendGrid.Helpers.Mail;

namespace Platform_Learning_Test.Controllers
{
    [Authorize(Roles = "Admin,Teacher")]
    [Route("Admin/AnswerOptions")]
    public class AnswerOptionsController : Controller
    {
        private readonly IAnswerService _answerService;
        private readonly IQuestionService _questionService;
        private readonly ILogger<AnswerOptionsController> _logger;
        private readonly ApplicationContext _context;

        private object testId;

        public AnswerOptionsController(
            IAnswerService answerService,
            IQuestionService questionService,
            ILogger<AnswerOptionsController> logger,
            ApplicationContext context)
        {
            _answerService = answerService;
            _questionService = questionService;
            _logger = logger;
            _context = context;
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
                ViewBag.TestId = question.TestId;
                return View(answers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting answers for question {questionId}");
                return View("Error");
            }
        }

        [HttpGet("Create")]
        public IActionResult Create(int questionId = 0)
        {
            ViewBag.Questions = _context.Questions.Include(q => q.Test).ToList();
            ViewBag.QuestionId = questionId;

            var model = new CreateAnswerOptionDto { QuestionId = questionId };
            return View(model);
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAnswerOptionDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Questions = await _context.Questions
                    .Include(q => q.Test)
                    .ToListAsync();
                return View(dto);
            }

            try
            {
               
                var questionExists = await _context.Questions.AnyAsync(q => q.Id == dto.QuestionId);
                if (!questionExists)
                {
                    ModelState.AddModelError("QuestionId", "Выбранный вопрос не существует");
                    ViewBag.Questions = await _context.Questions
                        .Include(q => q.Test)
                        .ToListAsync();
                    return View(dto);
                }

                var answer = new AnswerOption
                {
                    Text = dto.Text,
                    IsCorrect = dto.IsCorrect,
                    QuestionId = dto.QuestionId,
                    Explanation = string.Empty
                };

                _context.AnswerOptions.Add(answer);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "AnswerOptions", new { questionId = dto.QuestionId });
            }
            catch (DbUpdateException dbEx)
            {
                
                var errorMessage = "Ошибка при сохранении в базе данных";

              
                if (dbEx.InnerException != null)
                {
                    errorMessage += ": " + dbEx.InnerException.Message;
                }

                _logger.LogError(dbEx, "Database error while creating answer option");
                ModelState.AddModelError("", errorMessage);
                ViewBag.Questions = await _context.Questions
                    .Include(q => q.Test)
                    .ToListAsync();
                return View(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating answer option");
                ModelState.AddModelError("", $"Непредвиденная ошибка: {ex.Message}");
                ViewBag.Questions = await _context.Questions
                    .Include(q => q.Test)
                    .ToListAsync();
                return View(dto);
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
                return RedirectToAction("Index", "AdminDashboard");
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
                ViewBag.TestId = answer.TestId;
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
                if (answer == null) return NotFound();

                await _answerService.DeleteAnswerAsync(id);
                return RedirectToAction("Index", "AdminDashboard");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting answer with id {id}");
                return View("Error");
            }
        }

    }
}