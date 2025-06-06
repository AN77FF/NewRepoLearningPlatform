using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Platform_Learning_Test.Domain.Entities
{
    public class UserRole : IdentityUserRole<int>
    {
        public  User User { get; set; }
        public  Role Role { get; set; }
    }
}
