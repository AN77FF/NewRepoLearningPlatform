using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform_Learning_Test.Domain.Entities
{
    public class UserAnswer
    {
        public int Id { get; set; }
        public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;

        public int UserTestHistoryId { get; set; }
        public UserTestHistory UserTestHistory { get; set; }

        public int QuestionId { get; set; }
        public Question Question { get; set; }

        public int AnswerOptionId { get; set; }
        public AnswerOption AnswerOption { get; set; }
    }

}
