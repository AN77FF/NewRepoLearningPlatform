using System;
using System.Collections.Generic;

namespace Platform_Learning_Test.Domain.Dto
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string NormalizedEmail { get; set; } 
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; } 
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<RoleDto> Roles { get; set; } = new List<RoleDto>();
    }
}