using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform_Learning_Test.Domain.Dto
{
    public class UserAnswerDto
    {
        public int TestId { get; set; }
        public int QuestionId { get; set; }
        public int AnswerOptionId { get; set; }
        public DateTime AnswerTime { get; set; }
        public List<int>? SelectedAnswerIds { get; set; }
    }

}
