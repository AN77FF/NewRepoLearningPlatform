using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Platform_Learning_Test.Domain.Dto;

namespace Platform_Learning_Test.Domain.Dto
{
    public class TestResultDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public int TestId { get; set; }
        public string TestTitle { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public double ScorePercentage { get; set; }
        public DateTime CompletedAt { get; set; }
        public int Score { get; set; }
        public List<QuestionResultDto> Results { get; set; }
        public DateTime CompletionDate { get; set; }
    }

}
