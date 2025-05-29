using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Platform_Learning_Test.Data.Factory;
using Platform_Learning_Test.Domain.Dto;
using Platform_Learning_Test.Domain.Entities;

namespace Platform_Learning_Test.Service.Service
{
    public class TestReviewService : ITestReviewService
    {
        private readonly ApplicationContext _context;
        private readonly IMapper _mapper;

        public TestReviewService(ApplicationContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<TestReviewDto> GetReviewAsync(int id)
        {
            var review = await _context.TestReviews
                .Include(r => r.User)
                .Include(r => r.Test)
                .FirstOrDefaultAsync(r => r.Id == id);

            return _mapper.Map<TestReviewDto>(review);
        }

        public async Task<IEnumerable<TestReviewDto>> GetReviewsForTestAsync(int testId)
        {
            var reviews = await _context.TestReviews
                .Include(r => r.User)
                .Where(r => r.TestId == testId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<TestReviewDto>>(reviews);
        }

        public async Task<TestReviewDto> CreateReviewAsync(CreateTestReviewDto reviewDto, int userId)
        {
            var review = _mapper.Map<TestReview>(reviewDto);
            review.UserId = userId;

            await _context.TestReviews.AddAsync(review);
            await _context.SaveChangesAsync();

            return _mapper.Map<TestReviewDto>(review);
        }

        public async Task DeleteReviewAsync(int id)
        {
            var review = await _context.TestReviews.FindAsync(id);
            if (review != null)
            {
                _context.TestReviews.Remove(review);
                await _context.SaveChangesAsync();
            }
        }
    }


}
