using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform_Learning_Test.Domain.Entities
{
    public class AdminDashboardStats
    {
        public int TotalUsers { get; set; }
        public int TotalTests { get; set; }
        public int NewUsers { get; set; }
        public int ActiveTests { get; set; }
    }

}
