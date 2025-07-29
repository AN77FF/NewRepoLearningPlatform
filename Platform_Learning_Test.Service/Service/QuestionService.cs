using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Platform_Learning_Test.Data.Context;

using Platform_Learning_Test.Domain.Dto;
using Platform_Learning_Test.Domain.Entities;
using SendGrid.Helpers.Errors.Model;

namespace Platform_Learning_Test.Service.Service
{
    public class QuestionService : IQuestionService
    {
        private readonly ApplicationContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<QuestionService> _logger;

        public QuestionService(
            ApplicationContext context,
            IMapper mapper,
            ILogger<QuestionService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<QuestionDto> GetQuestionAsync(int id)
        {
            var question = await _context.Questions
                .Include(q => q.AnswerOptions)
                .FirstOrDefaultAsync(q => q.Id == id);

            return question == null
                ? throw new Exception($"Question with id {id} not found")
                : _mapper.Map<QuestionDto>(question);
        }

        public async Task<IEnumerable<QuestionDto>> GetAllQuestionsAsync()
        {
            return await _context.Questions
                .Include(q => q.Test) 
                .Select(q => new QuestionDto
                {
                    Id = q.Id,
                    Text = q.Text,
                    TestId = q.TestId,
                    TestTitle = q.Test.Title, 
                    Difficulty = q.Difficulty.ToString(),
                    TimeLimitSeconds = q.TimeLimitSeconds
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<QuestionDto>> GetQuestionsForTestAsync(int testId)
        {
            return await _context.Questions
                .Include(q => q.AnswerOptions)
                .Where(q => q.TestId == testId)
                .Select(q => _mapper.Map<QuestionDto>(q))
                .ToListAsync();
        }

        public async Task<QuestionDto> CreateQuestionAsync(CreateQuestionDto dto)
        {
            var question = new Question
            {
                Text = dto.Text,
                TestId = dto.TestId,
                Difficulty = QuestionDifficulty.Medium,
                TimeLimitSeconds = 60,
                AnswerOptions = dto.AnswerOptions.Select(a => new AnswerOption
                {
                    Text = a.Text,
                    IsCorrect = a.IsCorrect
                }).ToList()
            };

            await _context.Questions.AddAsync(question);
            await _context.SaveChangesAsync();

            return new QuestionDto
            {
                Id = question.Id,
                Text = question.Text,
                TestId = question.TestId,
                Difficulty = question.Difficulty.ToString(),
                TimeLimitSeconds = question.TimeLimitSeconds,
                AnswerOptions = question.AnswerOptions.Select(a => new AnswerOptionDto
                {
                    Id = a.Id,
                    Text = a.Text,
                    IsCorrect = a.IsCorrect
                }).ToList()
            };
        }

        public async Task UpdateQuestionAsync(UpdateQuestionDto dto)
        {
            var question = await _context.Questions
                .Include(q => q.AnswerOptions)
                .FirstOrDefaultAsync(q => q.Id == dto.Id)
                ?? throw new Exception($"Question with id {dto.Id} not found");

            question.Text = dto.Text;
            question.Difficulty = Enum.Parse<QuestionDifficulty>(dto.Difficulty);
            question.TimeLimitSeconds = dto.TimeLimitSeconds;

          
            foreach (var answerDto in dto.AnswerOptions)
            {
                if (answerDto.Id.HasValue)
                {
                    var answer = question.AnswerOptions.FirstOrDefault(a => a.Id == answerDto.Id.Value);
                    if (answer != null)
                    {
                        answer.Text = answerDto.Text;
                        answer.IsCorrect = answerDto.IsCorrect;
                    }
                }
                else
                {
                    question.AnswerOptions.Add(new AnswerOption
                    {
                        Text = answerDto.Text,
                        IsCorrect = answerDto.IsCorrect,
                        QuestionId = dto.Id
                    });
                }
            }

            
            var existingIds = dto.AnswerOptions.Where(a => a.Id.HasValue).Select(a => a.Id.Value);
            var toRemove = question.AnswerOptions.Where(a => !existingIds.Contains(a.Id)).ToList();
            foreach (var answer in toRemove)
            {
                question.AnswerOptions.Remove(answer);
                _context.AnswerOptions.Remove(answer);
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteQuestionAsync(int id)
        {
            var question = await _context.Questions.FindAsync(id)
                 ?? throw new Exception($"Question with id {id} not found");

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
        }

        public async Task ValidateQuestionAnswersAsync(int questionId, List<int> selectedAnswerIds)
        {
            var correctAnswers = await _context.AnswerOptions
                .Where(a => a.QuestionId == questionId && a.IsCorrect)
                .Select(a => a.Id)
                .ToListAsync();

            if (!correctAnswers.Any())
            {
                throw new InvalidOperationException("Question has no correct answers");
            }

            var isValid = correctAnswers.Count == selectedAnswerIds.Count &&
                          correctAnswers.All(id => selectedAnswerIds.Contains(id));

            if (!isValid)
            {
                throw new ValidationException("Selected answers are incorrect");
            }
        }
        public async Task AddAnswerOptionsAsync(int questionId, List<CreateAnswerOptionDto> answerOptions)
        {
            var question = await _context.Questions
                .Include(q => q.AnswerOptions)
                .FirstOrDefaultAsync(q => q.Id == questionId);

            if (question == null) throw new Exception("Question not found");

            foreach (var answerDto in answerOptions)
            {
                question.AnswerOptions.Add(new AnswerOption
                {
                    Text = answerDto.Text,
                    IsCorrect = answerDto.IsCorrect,
                    QuestionId = questionId
                });
            }

            await _context.SaveChangesAsync();
        }
    }
}
