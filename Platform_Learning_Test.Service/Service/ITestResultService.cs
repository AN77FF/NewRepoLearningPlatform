using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Platform_Learning_Test.Domain.Dto;

namespace Platform_Learning_Test.Service.Service
{
    public interface ITestResultService
    {
        Task<TestResultDto> SubmitTestAsync(SubmitTestDto submitDto, int userId);
        Task<IEnumerable<TestResultDto>> GetUserTestResultsAsync(int testId, int userId);
        Task<TestResultDetailsDto> GetTestResultDetailsAsync(int resultId);
        Task<IEnumerable<TestResultDto>> GetUserResultsAsync(int userId);
    }

}
