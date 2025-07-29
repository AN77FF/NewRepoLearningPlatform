using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Platform_Learning_Test.Data.Context;
using Platform_Learning_Test.Domain.Dto;
using Platform_Learning_Test.Models;
using Platform_Learning_Test.Service.Service;
using System.Diagnostics;

namespace Platform_Learning_Test.Controllers
{
    [Authorize(Roles = "Admin,Teacher")]
    [Route("Admin/Dashboard")]
    public class AdminDashboardController : Controller
    {
        private readonly ITestService _testService;
        private readonly IQuestionService _questionService;
        private readonly IAnswerService _answerService;
        private readonly ApplicationContext _context;

        public AdminDashboardController(
            ITestService testService,
            IQuestionService questionService,
            IAnswerService answerService,
            ApplicationContext context)
        {
            _testService = testService;
            _questionService = questionService;
            _answerService = answerService;
            _context = context;
        }

        [HttpGet("Dashboard")]
        public async Task<IActionResult> Index()
        {
            var stats = await _testService.GetDashboardStatsAsync();
            var tests = await _testService.GetAllTestsAsync();
            var questions = await _questionService.GetAllQuestionsAsync();
            var answers = await _answerService.GetAllAnswerOptionsAsync();

            var model = new AdminDashboardViewModel
            {
                Stats = stats,
                Tests = tests,
                Questions = questions,
                Answers = answers
            };

            return View(model);
        }
        

    }
}