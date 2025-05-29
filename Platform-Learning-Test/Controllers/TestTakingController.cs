using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Platform_Learning_Test.Domain.Dto;
using Platform_Learning_Test.Service.Service;

namespace Platform_Learning_Test.Controllers
{
    public class TestTakingController : Controller
    {
        private readonly ITestService _testService;
        private readonly ITestResultService _testResultService;

        public TestTakingController(
            ITestService testService,
            ITestResultService testResultService)
        {
            _testService = testService;
            _testResultService = testResultService;
        }

        [Authorize]
        [HttpGet("Test/{testId}/Start")]
        public async Task<IActionResult> StartTest(int testId)
        {
            var test = await _testService.GetTestWithDetailsAsync(testId);
            return View(test);
        }

        [HttpPost("Test/{testId}/Submit")]
        public async Task<IActionResult> SubmitTest(int testId, SubmitTestDto submitDto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _testResultService.SubmitTestAsync(submitDto, userId);
            return RedirectToAction("Details", "TestResults", new { id = result.Id });
        }
    }
}
