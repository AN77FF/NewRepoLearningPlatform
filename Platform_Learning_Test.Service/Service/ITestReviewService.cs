using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Platform_Learning_Test.Domain.Dto;

namespace Platform_Learning_Test.Service.Service
{
    public interface ITestReviewService
    {
        Task<TestReviewDto> GetReviewAsync(int id);
        Task<IEnumerable<TestReviewDto>> GetReviewsForTestAsync(int testId);
        Task<TestReviewDto> CreateReviewAsync(CreateTestReviewDto reviewDto, int userId);
        Task DeleteReviewAsync(int id);
    }

}
