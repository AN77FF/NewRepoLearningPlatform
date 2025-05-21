using Platform_Learning_Test.Domain.Entities;
using Platform_Learning_Test.Domain.Dto;
using System.Linq;

namespace Platform_Learning_Test.Domain.Extensions
{
    public static class UserExtensions
    {
        public static UserResponseDto ToDto(this User user)
        {
            if (user == null)
                return null;

            return new UserResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                NormalizedEmail = user.NormalizedEmail, 
                UserName = user.UserName,
                NormalizedUserName = user.NormalizedUserName, 
                Name = user.Name,
                CreatedAt = user.CreatedAt,
                Roles = user.UserRoles?
                    .Select(ur => ur.Role.ToDto())
                    .ToList() ?? new List<RoleDto>()
            };
        }

        public static User ToEntity(this UserDto userDto)
        {
            if (userDto == null)
                return null;

            return new User
            {
                UserName = userDto.Email, 
                Email = userDto.Email,
                NormalizedEmail = userDto.Email?.ToUpperInvariant(), 
                NormalizedUserName = userDto.Email?.ToUpperInvariant(), 
                Name = userDto.Name
            };
        }
    }
}