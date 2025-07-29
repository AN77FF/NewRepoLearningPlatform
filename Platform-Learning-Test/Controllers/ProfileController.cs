using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Platform_Learning_Test.Domain.Dto;
using Platform_Learning_Test.Service.Service;
using Platform_Learning_Test.Models.Account;
using Microsoft.AspNetCore.Identity;
using Platform_Learning_Test.Domain.Entities;

namespace Platform_Learning_Test.Controllers
{
    [Authorize]
    [Route("Profile")]
    public class ProfileController : Controller
    {
        private readonly IProfileService _profileService;
        private readonly ITestResultService _testResultService;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public ProfileController(
            IProfileService profileService,
            ITestResultService testResultService, 
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _profileService = profileService;
            _testResultService = testResultService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet("")] 
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var profile = await _profileService.GetUserProfileAsync(userId);
            var testResults = await _testResultService.GetUserResultsAsync(userId);

            var model = new ProfileModel
            {
                Profile = profile,
                TestResults = testResults
            };

            return View(model);
        }

        [HttpGet("Edit")]
        public async Task<IActionResult> Edit()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var profile = await _profileService.GetUserProfileAsync(userId);
            return View(profile);
        }

        [HttpPost("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateUserProfileDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _profileService.UpdateUserProfileAsync(userId, dto);

            
            var user = await _userManager.GetUserAsync(User);
            await _signInManager.RefreshSignInAsync(user);

            return RedirectToAction(nameof(Index));
        }
    }
}

