using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Platform_Learning_Test.Data.Context;
using Platform_Learning_Test.Domain.Dto;
using Platform_Learning_Test.Domain.Entities;
using SendGrid.Helpers.Errors.Model;
using Platform_Learning_Test.Domain.Extensions;

namespace Platform_Learning_Test.Service.Service
{
    public class TestService : ITestService
    {
        private readonly ApplicationContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<TestService> _logger;

        public TestService(
            ApplicationContext context,
            IMapper mapper,
            ILogger<TestService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<TestDetailDto> GetTestWithDetailsAsync(int id)
        {
            var test = await _context.Tests
                 .Include(t => t.Questions)
                     .ThenInclude(q => q.AnswerOptions)
                 .FirstOrDefaultAsync(t => t.Id == id);

            return test == null
                ? throw new Exception($"Test with id {id} not found")
                : _mapper.Map<TestDetailDto>(test);
        }

        public async Task<IEnumerable<TestDto>> GetAllTestsAsync()
        {
            return await _context.Tests
                .Include(t => t.Questions) 
                .Select(t => new TestDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Difficulty = t.Difficulty.GetDisplayName(),
                    Category = t.Category,
                    CreatedAt = t.CreatedAt,
                    Duration = t.Duration,
                    ImageUrl = t.ImageUrl,
                    QuestionCount = t.Questions.Count 
                })
                .ToListAsync();
        }

        public async Task<TestDto> CreateTestAsync(Test test)
        {
            try
            {
                await _context.Tests.AddAsync(test);
                await _context.SaveChangesAsync();
                return _mapper.Map<TestDto>(test);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating test");
                throw;
            }
        }

        public async Task<AdminDashboardStatsDto> GetDashboardStatsAsync()
        {
            var stats = new AdminDashboardStats
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalTests = await _context.Tests.CountAsync(),
                NewUsers = await _context.Users.CountAsync(u => u.CreatedAt > DateTime.UtcNow.AddDays(-7)),
                ActiveTests = await _context.Tests.CountAsync(t => t.IsFeatured)
            };

            return stats.ToDto();
        }
        public async Task<IEnumerable<TestDto>> GetFeaturedTestsAsync()
        {
            return await _context.Tests
                .Where(t => t.IsFeatured)
                .OrderByDescending(t => t.CreatedAt)
                .Take(4)
                .Select(t => _mapper.Map<TestDto>(t))
                .ToListAsync();
        }

        public async Task UpdateTestAsync(UpdateTestDto dto)
        {
            var test = await _context.Tests.FindAsync(dto.Id)
                ?? throw new Exception($"Test with id {dto.Id} not found");

            _mapper.Map(dto, test);
            await _context.SaveChangesAsync();
        }
        
        public async Task DeleteTestAsync(int id)
        {
            var test = await _context.Tests.FindAsync(id)
                 ?? throw new Exception($"Test with id {id} not found");

            _context.Tests.Remove(test);
            await _context.SaveChangesAsync();
        }
        public async Task CreateQuestionAsync(CreateQuestionDto dto)
        {
            var question = _mapper.Map<Question>(dto);
            await _context.Questions.AddAsync(question);
            await _context.SaveChangesAsync();
        }

        public async Task CreateAnswerOptionAsync(CreateAnswerOptionDto dto)
        {
            var option = _mapper.Map<AnswerOption>(dto);
            await _context.AnswerOptions.AddAsync(option);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteQuestionAsync(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question == null) throw new NotFoundException();

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAnswerOptionAsync(int id)
        {
            var option = await _context.AnswerOptions.FindAsync(id);
            if (option == null) throw new NotFoundException();

            _context.AnswerOptions.Remove(option);
            await _context.SaveChangesAsync();
        }
        public async Task<Test> GetTestAsync(int testId)
        {
            return await _context.Tests
                .Include(t => t.Questions)
                    .ThenInclude(q => q.AnswerOptions)
                .FirstOrDefaultAsync(t => t.Id == testId)
                ?? throw new NotFoundException($"Test with id {testId} not found");
        }

        public async Task<TestResultDto> SaveTestResultsAsync(int testId,string userId,List<UserAnswerDto> answers)
        {
            var test = await GetTestAsync(testId);

            int score = 0;
            var results = new List<QuestionResultDto>();

            foreach (var question in test.Questions)
            {
                var userAnswer = answers.FirstOrDefault(a => a.QuestionId == question.Id);
                var correctAnswerIds = question.AnswerOptions
                    .Where(a => a.IsCorrect)
                    .Select(a => a.Id)
                    .ToList();

                bool isCorrect = userAnswer != null &&
                    correctAnswerIds.Count == userAnswer.SelectedAnswerIds.Count &&
                    correctAnswerIds.All(id => userAnswer.SelectedAnswerIds.Contains(id));

                if (isCorrect) score++;

                results.Add(new QuestionResultDto
                {
                    QuestionId = question.Id,
                    IsCorrect = isCorrect,
                    SelectedAnswers = userAnswer?.SelectedAnswerIds ?? new List<int>(),
                    CorrectAnswers = correctAnswerIds
                });
            }

            var testResult = new TestResult
            {
                TestId = testId,
                UserId = userId,
                Score = score,
                TotalQuestions = test.Questions.Count,
                CompletionDate = DateTime.UtcNow
            };

            _context.TestResults.Add(testResult);
            await _context.SaveChangesAsync();

           
            return new TestResultDto
            {
                TestId = testId,
                TestTitle = test.Title,
                Score = score,
                TotalQuestions = test.Questions.Count,
                Results = results
            };
        }

        public async Task<TestResultDto> GetTestResultAsync(int resultId)
        {
            var result = await _context.TestResults
                .Include(r => r.Test)
                .FirstOrDefaultAsync(r => r.Id == resultId)
                ?? throw new NotFoundException($"Test result with id {resultId} not found");

            return new TestResultDto
            {
                Id = result.Id,
                TestId = result.TestId,
                TestTitle = result.Test.Title,
                Score = result.Score,
                TotalQuestions = result.TotalQuestions,
                CompletionDate = result.CompletionDate
            };
        }
    }
}

