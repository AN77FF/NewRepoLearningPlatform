using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform_Learning_Test.Domain.Dto
{
    public class QuestionDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int TestId { get; set; }
        public string Difficulty { get; set; }
        public int TimeLimitSeconds { get; set; }
        public string TestTitle { get; set; }
        public List<AnswerOptionDto> AnswerOptions { get; set; } = new();
    }

}
