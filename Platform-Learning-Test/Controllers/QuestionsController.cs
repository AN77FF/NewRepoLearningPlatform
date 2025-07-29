using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Platform_Learning_Test.Data.Context;
using Platform_Learning_Test.Domain.Dto;
using Platform_Learning_Test.Domain.Entities;
using Platform_Learning_Test.Service.Service;
using Platform_Learning_Test.Domain.Extensions;

namespace Platform_Learning_Test.Controllers
{
    [Authorize(Roles = "Admin,Teacher")]
    [Route("Admin/Questions")]
    public class QuestionsController : Controller
    {
        private readonly IQuestionService _questionService;
        private readonly ITestService _testService;
        private readonly ILogger<QuestionsController> _logger;
        private readonly ApplicationContext _context;

        public QuestionsController(
            IQuestionService questionService,
            ITestService testService,
            ILogger<QuestionsController> logger,
            ApplicationContext context)
        {
            _questionService = questionService;
            _testService = testService;
            _logger = logger;
            _context = context;
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

        [HttpGet("Create")]
        public IActionResult Create(int testId = 0)
        {
            ViewBag.Tests = _context.Tests.ToList();
            ViewBag.TestId = testId;

            var model = new CreateQuestionDto { TestId = testId };
            return View(model);
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateQuestionDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Tests = await _context.Tests.ToListAsync();
                return View(dto);
            }

            try
            {
                var test = await _context.Tests.FindAsync(dto.TestId);
                if (test == null)
                {
                    ModelState.AddModelError("", "Указанный тест не существует");
                    ViewBag.Tests = await _context.Tests.ToListAsync();
                    return View(dto);
                }

                
                var question = new Question
                {
                    Text = dto.Text,
                    TestId = dto.TestId,
                    Difficulty = Enum.Parse<QuestionDifficulty>(dto.Difficulty),
                    TimeLimitSeconds = dto.TimeLimitSeconds
                };

             
                foreach (var answerDto in dto.AnswerOptions)
                {
                    question.AnswerOptions.Add(new AnswerOption
                    {
                        Text = answerDto.Text,
                        IsCorrect = answerDto.IsCorrect
                    });
                }

                await _context.Questions.AddAsync(question);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "AdminDashboard");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating question");
                ModelState.AddModelError("", $"Ошибка: {ex.Message}");
                ViewBag.Tests = await _context.Tests.ToListAsync();
                return View(dto);
            }
        }
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var question = await _context.Questions
                    .Include(q => q.AnswerOptions)
                    .FirstOrDefaultAsync(q => q.Id == id);

                if (question == null) return NotFound();

                return View(new UpdateQuestionDto
                {
                    Id = question.Id,
                    Text = question.Text,
                    Difficulty = question.Difficulty.ToString(),
                    TimeLimitSeconds = question.TimeLimitSeconds,
                    TestId = question.TestId,
                    AnswerOptions = question.AnswerOptions.Select(a => new UpdateAnswerOptionDto
                    {
                        Id = a.Id,
                        Text = a.Text,
                        IsCorrect = a.IsCorrect
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting question for edit with id {id}");
                return View("Error");
            }
        }
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateQuestionDto dto)
        {
            if (id != dto.Id) return NotFound();

            if (!ModelState.IsValid) return View(dto);

            try
            {
                var question = await _context.Questions
                    .Include(q => q.AnswerOptions)
                    .FirstOrDefaultAsync(q => q.Id == id);

                if (question == null) return NotFound();

                
                question.Text = dto.Text;
                question.Difficulty = Enum.Parse<QuestionDifficulty>(dto.Difficulty);
                question.TimeLimitSeconds = dto.TimeLimitSeconds;
                question.TestId = dto.TestId;

               
                foreach (var answerDto in dto.AnswerOptions)
                {
                    var existingAnswer = question.AnswerOptions
                        .FirstOrDefault(a => a.Id == answerDto.Id);

                    if (existingAnswer != null)
                    {
                        existingAnswer.Text = answerDto.Text;
                        existingAnswer.IsCorrect = answerDto.IsCorrect;
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "AdminDashboard");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating question {id}");
                ModelState.AddModelError("", $"Ошибка: {ex.Message}");
                return View(dto);
            }
        }
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var question = await _questionService.GetQuestionAsync(id);
                if (question == null) return NotFound();

                ViewBag.TestId = question.TestId;
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
                await _questionService.DeleteQuestionAsync(id);
                return RedirectToAction("Index", "AdminDashboard");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting question with id {id}");
                return View("Error");
            }
        }

    }
}
