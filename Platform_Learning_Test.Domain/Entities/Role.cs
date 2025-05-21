using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Platform_Learning_Test.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Platform_Learning_Test.Domain.Entities
{
    public class Role : IdentityRole<int>
    {
        public int Id { get; set; }

        [Required]
        [StringLength(256)]
        public string Name { get; set; }

        [Required]
        [StringLength(256)]
        public string NormalizedName { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
