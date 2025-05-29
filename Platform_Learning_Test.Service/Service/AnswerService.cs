using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Platform_Learning_Test.Data.Factory;
using Platform_Learning_Test.Domain.Dto;
using Platform_Learning_Test.Domain.Entities;
using SendGrid.Helpers.Errors.Model;

namespace Platform_Learning_Test.Service.Service
{
    public class AnswerService : IAnswerService
    {
        private readonly ApplicationContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<AnswerService> _logger;

        public AnswerService(
            ApplicationContext context,
            IMapper mapper,
            ILogger<AnswerService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<AnswerOptionDto> GetAnswerAsync(int id)
        {
            var answer = await _context.AnswerOptions
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);

            if (answer == null)
            {
                _logger.LogWarning("Answer with id {AnswerId} not found", id);
                throw new NotFoundException($"Answer with id {id} not found");
            }

            return _mapper.Map<AnswerOptionDto>(answer);
        }

        public async Task<IEnumerable<AnswerOptionDto>> GetAnswersForQuestionAsync(int questionId)
        {
            var answers = await _context.AnswerOptions
                .AsNoTracking()
                .Where(a => a.QuestionId == questionId)
                .OrderBy(a => a.Id)
                .ToListAsync();

            return _mapper.Map<IEnumerable<AnswerOptionDto>>(answers);
        }

        public async Task<AnswerOptionDto> CreateAnswerAsync(CreateAnswerOptionDto dto)
        {
            if (!await _context.Questions.AnyAsync(q => q.Id == dto.QuestionId))
            {
                throw new ValidationException($"Question with id {dto.QuestionId} does not exist");
            }

            var answer = _mapper.Map<AnswerOption>(dto);

            await _context.AnswerOptions.AddAsync(answer);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created new answer with id {AnswerId} for question {QuestionId}",
                answer.Id, answer.QuestionId);

            return _mapper.Map<AnswerOptionDto>(answer);
        }

        public async Task UpdateAnswerAsync(UpdateAnswerOptionDto dto)
        {
            var answer = await _context.AnswerOptions.FindAsync(dto.Id);
            if (answer == null)
            {
                _logger.LogWarning("Answer with id {AnswerId} not found for update", dto.Id);
                throw new NotFoundException($"Answer with id {dto.Id} not found");
            }

            _mapper.Map(dto, answer);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Updated answer with id {AnswerId}", answer.Id);
        }

        public async Task DeleteAnswerAsync(int id)
        {
            var answer = await _context.AnswerOptions.FindAsync(id);
            if (answer == null)
            {
                _logger.LogWarning("Answer with id {AnswerId} not found for deletion", id);
                throw new NotFoundException($"Answer with id {id} not found");
            }

            _context.AnswerOptions.Remove(answer);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Deleted answer with id {AnswerId}", id);
        }

        public async Task<bool> IsAnswerCorrectAsync(int answerId)
        {
            var answer = await _context.AnswerOptions
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == answerId);

            if (answer == null)
            {
                throw new NotFoundException($"Answer with id {answerId} not found");
            }

            return answer.IsCorrect;
        }
    }
}
