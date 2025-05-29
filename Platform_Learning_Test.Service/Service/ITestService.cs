using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Platform_Learning_Test.Domain.Dto;

namespace Platform_Learning_Test.Service.Service
{
    public interface ITestService
    {
        Task<TestDetailDto> GetTestWithDetailsAsync(int id);
        Task<IEnumerable<TestDto>> GetAllTestsAsync();
        Task<TestDto> CreateTestAsync(CreateTestDto dto);
        Task UpdateTestAsync(UpdateTestDto dto);
        Task DeleteTestAsync(int id);
        

    }

}
