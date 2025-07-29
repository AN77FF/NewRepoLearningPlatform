using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Platform_Learning_Test.Data.Context.Factory;
using Platform_Learning_Test.Data.Context;
using Platform_Learning_Test.Domain.Dto;
using Platform_Learning_Test.Domain.Entities;
using Platform_Learning_Test.Service.Service;
using Platform_Learning_Test.Services.Stores;
using UserStore = Microsoft.AspNetCore.Identity.EntityFrameworkCore.UserStore;
using SendGrid.Helpers.Mail;
using Platform_Learning_Test.Common.Profiles;

using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.ModelBinding;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IApplicationContextFactory, ApplicationContextFactory>();
builder.Services.AddScoped<ITestService, TestService>();
builder.Services.AddScoped<ITestResultService, TestResultService>();
builder.Services.AddScoped<IQuestionService, QuestionService>();
builder.Services.AddScoped<IAnswerService, AnswerService>();
builder.Services.AddScoped<ITestReviewService, TestReviewService>();
builder.Services.AddScoped<IProfileService, ProfileService>();



builder.Services.AddIdentity<User, Role>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;

    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddEntityFrameworkStores<ApplicationContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "LearningPlatform.Auth";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
    options.ReturnUrlParameter = "returnUrl";
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("TeacherOnly", policy => policy.RequireRole("Teacher"));
    options.AddPolicy("ContentManagers", policy =>
        policy.RequireRole("Admin", "Teacher"));
});

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();



app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "admin",
        pattern: "Admin/{controller=Dashboard}/{action=Index}/{id?}");

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await InitializeDatabaseAsync(services);
}

await app.RunAsync();



async Task InitializeDatabaseAsync(IServiceProvider services)
{
    var context = services.GetRequiredService<ApplicationContext>();
    await context.Database.MigrateAsync();

    var roleManager = services.GetRequiredService<RoleManager<Role>>();
    var userManager = services.GetRequiredService<UserManager<User>>();


    string[] roleNames = { "Admin", "Teacher", "User" };
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new Role(roleName));
        }
    }

    var adminEmail = "admin@example.com";
    if (await userManager.FindByEmailAsync(adminEmail) == null)
    {
        var adminUser = new User
        {
            UserName = adminEmail,
            Email = adminEmail,
            Name = "Administrator",
            CreatedAt = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(adminUser, "Admin123!");
        if (result.Succeeded)
        {
            await userManager.AddToRolesAsync(adminUser, new[] { "Admin", "Teacher" });
        }
    }

    



if (!await context.Tests.AnyAsync())
{

    var tests = new List<Test>
        {
            new Test
            {
                Title = "������ Python",
                Description = "������� ������ ���������������� �� Python",
                Category = "����������������",
                Difficulty = TestDifficulty.Medium,
                Duration = TimeSpan.FromHours(12),
                ImageUrl = "https://placehold.co/600x400/4CAF50/FFFFFF?text=Python",
                CreatedAt = DateTime.UtcNow,
                IsFeatured = true,
                Questions = new List<Question>
                {
                    new Question
                    {
                        Text = "��� ����� ������ � Python?",
                        Difficulty = QuestionDifficulty.Easy,
                        AnswerOptions = new List<AnswerOption>
                        {
                            new AnswerOption { Text = "���������� ��������� ���������", IsCorrect = true, Explanation = "��� ���������� �����" },
                            new AnswerOption { Text = "������������ ��������� ���������", IsCorrect = false, Explanation = "��� �� ���������� �����" }
                        }
                    }
                }
            },
            new Test
            {
                Title = "���������� ��� ����������",
                Description = "���� ��� ���, ��� ������ �������� ������� ����������",
                Category = "�����",
                Difficulty = TestDifficulty.Easy,
                Duration = TimeSpan.FromHours(20),
                ImageUrl = "https://placehold.co/600x400/2196F3/FFFFFF?text=English",
                CreatedAt = DateTime.UtcNow,
                IsFeatured = true,
                Questions = new List<Question>
                {
                    new Question
                    {
                        Text = "��� ����������� 'apple'?",
                        Difficulty = QuestionDifficulty.Easy,
                        AnswerOptions = new List<AnswerOption>
                        {
                            new AnswerOption { Text = "������", IsCorrect = true, Explanation = "��� ���������� �����" },
                            new AnswerOption { Text = "��������", IsCorrect = false, Explanation = "��� �� ���������� �����" }
                        }
                    }
                }
            },
            new Test
            {
                Title = "��������� ������ � ����",
                Description = "������� �������� �� ����� ����",
                Category = "��������� ������",
                Difficulty = TestDifficulty.Medium,
                Duration = TimeSpan.FromHours(20),
                ImageUrl = "https://placehold.co/600x400/2196F3/FFFFFF?text=Analytics",
                CreatedAt = DateTime.UtcNow,
                IsFeatured = true,
                Questions = new List<Question>
                {
                    new Question
                    {
                        Text = "�� ������ ����� ������ ��������� �������� ������������� ��������� �������?",
                        Difficulty = QuestionDifficulty.Easy,
                        AnswerOptions = new List<AnswerOption>
                        {
                            new AnswerOption { Text = "SQL, Python, ROI, ���������, CPA ���.", IsCorrect = true, Explanation = "��� ���������� �����" },
                            new AnswerOption { Text = "����������� ��� ������ � ����������� �����������", IsCorrect = false, Explanation = "��� �� ���������� �����" }
                        }
                    }
                }
            },
            new Test
            {
                Title = "������ � Figma",
                Description = "�������� ������ � ����������� ��������� ��� �������� �����������, ������������� ���������� � ������ ������-��������",
                Category = "������",
                Difficulty = TestDifficulty.Easy,
                Duration = TimeSpan.FromHours(20),
                ImageUrl = "https://placehold.co/600x400/2196F3/FFFFFF?text=Figma",
                CreatedAt = DateTime.UtcNow,
                IsFeatured = true,
                Questions = new List<Question>
                {
                    new Question
                    {
                        Text = "����� �� �������� � Figma �������� ��� ��������� �������� (on scroll)?",
                        Difficulty = QuestionDifficulty.Easy,
                        AnswerOptions = new List<AnswerOption>
                        {
                            new AnswerOption { Text = "��, � Figma ����� �������� �������� ��� ��������� �������� (on scroll)", IsCorrect = true, Explanation = "��� ���������� �����" },
                            new AnswerOption { Text = "���, Figma �� ������������ ��� ��������", IsCorrect = false, Explanation = "��� �� ���������� �����" }
                        }
                    }
                }
            }
        };


        foreach (var test in tests)
        {
            context.Tests.Add(test);
        }
        await context.SaveChangesAsync();
    }
  
}
