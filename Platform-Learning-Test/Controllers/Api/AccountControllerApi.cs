using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Platform_Learning_Test.Domain.Dto;
using Platform_Learning_Test.Domain.Entities;

namespace Platform_Learning_Test.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountControllerApi : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountControllerApi> _logger;

        public AccountControllerApi(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration,
            ILogger<AccountControllerApi> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserResponseDto>> RegisterAsync([FromBody] UserDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var normalizedEmail = model.Email.ToUpperInvariant();
            var existingUser = await _userManager.Users
                .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);

            if (existingUser != null)
            {
                return BadRequest("User with this email already exists");
            }

            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                Name = model.Name,
                NormalizedEmail = normalizedEmail,
                NormalizedUserName = model.Email.ToUpperInvariant()
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));

            await _userManager.AddToRoleAsync(user, "User");

            _logger.LogInformation("User registered with email: {Email}", user.Email);

            return Ok(await CreateUserResponseDtoAsync(user));
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> LoginAsync([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var normalizedEmail = model.Email.ToUpperInvariant();
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);

            if (user == null)
                return Unauthorized("Invalid credentials");

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
                return Unauthorized("Invalid credentials");

            var token = GenerateJwtToken(user);
            _logger.LogInformation("User {Email} logged in", user.Email);

            var authResponse = new AuthResponseDto
            {
                Token = token,
                User = await CreateUserResponseDtoAsync(user)
            };

            return Ok(authResponse);
        }

        [HttpGet("userinfo")]
        [Authorize]
        public async Task<ActionResult<UserResponseDto>> GetUserInfoAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound("User not found");

            return Ok(await CreateUserResponseDtoAsync(user));
        }

        private async Task<UserResponseDto> CreateUserResponseDtoAsync(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            return new UserResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                CreatedAt = user.CreatedAt,
                Roles = roles.Select(r => new RoleDto { Name = r }).ToList()
            };
        }

        private string GenerateJwtToken(User user)
        {
            var now = DateTime.UtcNow;
            var expires = now.AddDays(Convert.ToDouble(_configuration["Jwt:ExpireDays"]));

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Name, user.Name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(now).ToUnixTimeSeconds().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString())
            };

            var roles = _userManager.GetRolesAsync(user).Result;
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"] ?? _configuration["Jwt:Issuer"],
                claims: claims,
                notBefore: now,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}