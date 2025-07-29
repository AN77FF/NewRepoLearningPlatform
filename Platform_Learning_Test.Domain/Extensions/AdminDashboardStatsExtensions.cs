using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Platform_Learning_Test.Domain.Dto;
using Platform_Learning_Test.Domain.Entities;

namespace Platform_Learning_Test.Domain.Extensions
{
    public static class AdminDashboardStatsExtensions
    {
        public static AdminDashboardStatsDto ToDto(this AdminDashboardStats stats)
        {
            return new AdminDashboardStatsDto
            {
                TotalUsers = stats.TotalUsers,
                TotalTests = stats.TotalTests,
                NewUsers = stats.NewUsers,
                ActiveTests = stats.ActiveTests
            };
        }
    }
}
