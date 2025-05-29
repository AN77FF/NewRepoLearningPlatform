using Platform_Learning_Test.Domain.Dto;
using Platform_Learning_Test.Domain.Entities;

namespace Platform_Learning_Test.Models.Account
{
    public class ProfileModel
    {
        public User User { get; set; }
        public IEnumerable<TestResultDto> TestResults { get; set; }
    }

}
