using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Platform_Learning_Test.Domain.Dto;

namespace Platform_Learning_Test.Service.Service
{
    public interface IProfileService
    {
        Task<UserProfileDto> GetUserProfileAsync(int userId);
        Task UpdateUserProfileAsync(int userId, UpdateUserProfileDto dto);
    }
}
