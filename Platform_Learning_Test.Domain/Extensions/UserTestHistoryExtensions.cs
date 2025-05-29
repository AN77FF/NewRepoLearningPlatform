using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Platform_Learning_Test.Domain.Dto;
using Platform_Learning_Test.Domain.Entities;

namespace Platform_Learning_Test.Domain.Extensions
{
    public static class UserTestHistoryExtensions
    {
        public static TestResultDto ToDto(this UserTestHistory history)
        {
            return new TestResultDto
            {
                Id = history.Id,
                TestId = history.TestId,
                TestTitle = history.Test?.Title ?? "Unknown Test",
                CompletedAt = history.CompletedAt ?? DateTime.UtcNow,
                ScorePercentage = history.Score ?? 0
            };
        }
    }
}
