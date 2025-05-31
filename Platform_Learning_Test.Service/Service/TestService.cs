using System;
using System.Collections.Generic;
using System.Linq;
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
                ? throw new NotFoundException($"Test with id {id} not found")
                : _mapper.Map<TestDetailDto>(test);
        }

        public async Task<IEnumerable<TestDto>> GetAllTestsAsync()
        {
            return await _context.Tests
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => _mapper.Map<TestDto>(t))
                .ToListAsync();
        }

        public async Task<TestDto> CreateTestAsync(CreateTestDto dto)
        {
            var test = _mapper.Map<Test>(dto);
            await _context.Tests.AddAsync(test);
            await _context.SaveChangesAsync();
            return _mapper.Map<TestDto>(test);
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
                ?? throw new NotFoundException($"Test with id {dto.Id} not found");

            _mapper.Map(dto, test);
            test.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTestAsync(int id)
        {
            var test = await _context.Tests.FindAsync(id)
                ?? throw new NotFoundException($"Test with id {id} not found");

            _context.Tests.Remove(test);
            await _context.SaveChangesAsync();
        }
        
    }
}