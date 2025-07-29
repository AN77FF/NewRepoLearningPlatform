using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Platform_Learning_Test.Domain.Dto;
using Platform_Learning_Test.Domain.Entities;

namespace Platform_Learning_Test.Service.Service
{
    public interface ITestService
    {
        Task<TestDetailDto> GetTestWithDetailsAsync(int id);
        Task<IEnumerable<TestDto>> GetAllTestsAsync();
        
        Task<IEnumerable<TestDto>> GetFeaturedTestsAsync();
        Task<AdminDashboardStatsDto> GetDashboardStatsAsync();
        Task UpdateTestAsync(UpdateTestDto dto);
       
        Task DeleteTestAsync(int id);
        Task CreateQuestionAsync(CreateQuestionDto dto);
        Task CreateAnswerOptionAsync(CreateAnswerOptionDto dto);
        Task DeleteQuestionAsync(int id);
        Task DeleteAnswerOptionAsync(int id);
        Task<Test> GetTestAsync(int testId);
        Task<TestResultDto> SaveTestResultsAsync(int testId, string userId, List<UserAnswerDto> answers);
        Task<TestResultDto> GetTestResultAsync(int resultId);
        Task<TestDto> CreateTestAsync(Test test);
      

  

    }

}
