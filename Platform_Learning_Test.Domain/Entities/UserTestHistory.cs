using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform_Learning_Test.Domain.Entities
{
    public class UserTestHistory
    {
        public int Id { get; set; }

        public TestStatus Status { get; set; }
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public int? Score { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int TestId { get; set; }
        public Test Test { get; set; }

        public ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
    }

    public enum TestStatus
    {
        InProgress,
        Completed
    }

}
