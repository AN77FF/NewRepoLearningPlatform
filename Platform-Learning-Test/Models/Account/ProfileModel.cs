using Platform_Learning_Test.Domain.Dto;
using Platform_Learning_Test.Domain.Entities;

namespace Platform_Learning_Test.Models.Account
{
    public class ProfileModel
    {
        public UserProfileDto Profile { get; set; }
        public IEnumerable<TestResultDto> TestResults { get; set; }
    }

}
