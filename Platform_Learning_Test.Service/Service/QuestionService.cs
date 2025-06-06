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
            if (!await _context.Tests.AnyAsync(t => t.Id == dto.TestId))
                throw new ValidationException($"Test with id {dto.TestId} does not exist");

            if (dto.AnswerOptions.Count < 2)
                throw new ValidationException("Question must have at least 2 answer options");

            if (!dto.AnswerOptions.Any(a => a.IsCorrect))
                throw new ValidationException("At least one answer option must be correct");

            var question = _mapper.Map<Question>(dto);
            await _context.Questions.AddAsync(question);
            await _context.SaveChangesAsync();

            return _mapper.Map<QuestionDto>(question);
        }

        public async Task UpdateQuestionAsync(UpdateQuestionDto dto)
        {
            var question = await _context.Questions
                .Include(q => q.AnswerOptions)
                .FirstOrDefaultAsync(q => q.Id == dto.Id)
                ?? throw new Exception($"Question with id {dto.Id} not found");

            _mapper.Map(dto, question);
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
    }
}
