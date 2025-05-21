using Platform_Learning_Test.Domain.Entities;
using Platform_Learning_Test.Domain.Dto;

namespace Platform_Learning_Test.Domain.Extensions
{
    public static class RoleExtensions
    {
        public static RoleDto ToDto(this Role role)
        {
            if (role == null)
                return null;

            return new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                NormalizedName = role.NormalizedName, 
                Description = role.Description
            };
        }

        public static Role ToEntity(this RoleDto roleDto)
        {
            if (roleDto == null)
                return null;

            return new Role
            {
                Id = roleDto.Id,
                Name = roleDto.Name,
                NormalizedName = roleDto.NormalizedName ?? roleDto.Name?.ToUpperInvariant(), 
                Description = roleDto.Description
            };
        }
    }
}