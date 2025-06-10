using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Platform_Learning_Test.Domain.Entities;
using Platform_Learning_Test.Models.Account;
using Platform_Learning_Test.Service.Service;
using Microsoft.Extensions.Logging;

namespace Platform_Learning_Test.Controllers.Account
{
    [Route("Account")]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IProfileService _profileService;
        private readonly ITestResultService _testResultService;

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<Role> roleManager,
            ILogger<AccountController> logger,
            IProfileService profileService,
            ITestResultService testResultService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
            _profileService = profileService;
            _testResultService = testResultService;
        }

        [HttpGet("Register")]

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("Register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAsync(RegisterModel model)
        {
            _logger.LogInformation("Начало регистрации для {Email}", model.Email);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Модель невалидна для {Email}", model.Email);
                return View(model);
            }

            try
            {

                var user = new User
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    Name = model.Name,
                    NormalizedEmail = model.Email.ToUpperInvariant(),
                    NormalizedUserName = model.UserName.ToUpperInvariant()
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Пользователь {Email} успешно зарегистрирован", model.Email);

                    await _userManager.AddToRoleAsync(user, "User");

                    await _signInManager.SignInAsync(user, isPersistent: false, "Identity.Application");

                    _logger.LogInformation("Перенаправление в профиль");

                    return RedirectToAction("Index", "Home");
                }
                _logger.LogError("Ошибки при регистрации: {@Errors}", result.Errors);
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Критическая ошибка при регистрации");
                ModelState.AddModelError(string.Empty, "Произошла критическая ошибка при регистрации");
            }


            return View(model);
        }


        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginAsync(LoginModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByNameAsync(model.UserName);

            if (user == null)

            {

                ModelState.AddModelError(string.Empty, "Неверный логин или пароль");

                return View(model);

            }

            var result = await _signInManager.PasswordSignInAsync(

                model.UserName,

                model.Password,

                model.RememberMe,

                lockoutOnFailure: false);

            if (result.Succeeded)

            {

                return RedirectToAction("Index", "Profile");

            }

            ModelState.AddModelError(string.Empty, "Неверный логин или пароль");

            return View(model);

        }

        [HttpPost("Logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("ForgotPassword")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost("ForgotPassword")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPasswordAsync(ForgotPasswordModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var normalizedEmail = model.Email.ToUpperInvariant();
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);

            if (user == null)
            {
                return RedirectToAction("ForgotPasswordConfirmation");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action(
                "ResetPassword",
                "Account",
                new { email = user.Email, token },
                protocol: HttpContext.Request.Scheme
            );

            _logger.LogInformation("Password reset link: {CallbackUrl}", callbackUrl);

            return RedirectToAction("ForgotPasswordConfirmation");
        }

        [HttpGet("ForgotPasswordConfirmation")]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

    }
}