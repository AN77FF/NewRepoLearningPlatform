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
        public Role() { }

        public Role(string name) : base(name)
        {
            NormalizedName = name.ToUpperInvariant();
        }



        [StringLength(200)]
        public string Description { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
