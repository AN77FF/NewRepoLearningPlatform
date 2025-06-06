using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Platform_Learning_Test.Data.Context;
using Platform_Learning_Test.Domain.Dto;
using Platform_Learning_Test.Domain.Entities;
using SendGrid.Helpers.Errors.Model;

namespace Platform_Learning_Test.Service.Service
{
    public class TestResultService : ITestResultService
    {
        private readonly ApplicationContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<TestResultService> _logger;
        private readonly ITestService _testService;

        public TestResultService(
            ApplicationContext context,
            IMapper mapper,
            ILogger<TestResultService> logger,
            ITestService testService)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _testService = testService;
        }

        public async Task<TestResultDto> SubmitTestAsync(SubmitTestDto submitDto, int userId)
        {
            var test = await _testService.GetTestWithDetailsAsync(submitDto.TestId);

            ValidateAnswers(submitDto, test);

            var history = new UserTestHistory
            {
                UserId = userId,
                TestId = submitDto.TestId,
                StartedAt = DateTime.UtcNow,
                Status = TestStatus.Completed,
                CompletedAt = DateTime.UtcNow
            };


            foreach (var answer in submitDto.Answers)
            {
                history.UserAnswers.Add(new UserAnswer
                {
                    QuestionId = answer.QuestionId,
                    AnswerOptionId = answer.AnswerOptionId,
                    AnsweredAt = DateTime.UtcNow
                });
            }


            CalculateResults(history, test);

            await _context.UserTestHistories.AddAsync(history);
            await _context.SaveChangesAsync();

            return _mapper.Map<TestResultDto>(history);
        }

        private void ValidateAnswers(SubmitTestDto submitDto, TestDetailDto test)
        {

            foreach (var answer in submitDto.Answers)
            {
                var question = test.Questions.FirstOrDefault(q => q.Id == answer.QuestionId);
                if (question == null)
                    throw new ValidationException($"Question {answer.QuestionId} not found in test");

                if (!question.AnswerOptions.Any(a => a.Id == answer.AnswerOptionId))
                    throw new ValidationException($"Invalid answer option {answer.AnswerOptionId} for question {answer.QuestionId}");
            }
        }

        private void CalculateResults(UserTestHistory history, TestDetailDto test)
        {
            int correctAnswers = 0;
            foreach (var userAnswer in history.UserAnswers)
            {
                var question = test.Questions.First(q => q.Id == userAnswer.QuestionId);
                var answer = question.AnswerOptions.First(a => a.Id == userAnswer.AnswerOptionId);

                if (answer.IsCorrect)
                    correctAnswers++;
            }

            history.Score = (int)Math.Round((double)correctAnswers / test.Questions.Count * 100);
        }

        public async Task<IEnumerable<TestResultDto>> GetUserTestResultsAsync(int testId, int userId)
        {
            return await _context.UserTestHistories
                .Where(h => h.TestId == testId && h.UserId == userId)
                .OrderByDescending(h => h.CompletedAt)
                .ProjectTo<TestResultDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<TestResultDetailsDto> GetTestResultDetailsAsync(int resultId)
        {
            var history = await _context.UserTestHistories
                .Include(h => h.UserAnswers)
                .ThenInclude(a => a.AnswerOption)
                .FirstOrDefaultAsync(h => h.Id == resultId);

            if (history == null)
                throw new NotFoundException("Test result not found");

            return _mapper.Map<TestResultDetailsDto>(history);
        }
        public async Task<IEnumerable<TestResultDto>> GetUserResultsAsync(int userId)
        {
            return await _context.UserTestHistories
                .Where(h => h.UserId == userId)
                .ProjectTo<TestResultDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }
    }

}
