using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform_Learning_Test.Domain.Dto
{
        public class UserProfileDto
        {
            public int Id { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public string Name { get; set; }
            public string? Bio { get; set; }
            public string? Location { get; set; }
            public string? AvatarUrl { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        public class UpdateUserProfileDto
        {
            [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
            public string Name { get; set; }

            [StringLength(500, ErrorMessage = "Bio cannot be longer than 500 characters.")]
            public string? Bio { get; set; }

            [StringLength(200, ErrorMessage = "Location cannot be longer than 200 characters.")]
            public string? Location { get; set; }

            [StringLength(300, ErrorMessage = "Avatar URL cannot be longer than 300 characters.")]
            [Url(ErrorMessage = "Please enter a valid URL.")]
            public string? AvatarUrl { get; set; }
        }
    
}
