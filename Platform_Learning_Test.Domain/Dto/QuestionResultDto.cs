using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform_Learning_Test.Domain.Dto
{
    public class QuestionResultDto
    {
        public string QuestionText { get; set; }
        public bool IsCorrect { get; set; }
        public string SelectedAnswer { get; set; }
        public string CorrectAnswer { get; set; }
        public int QuestionId { get; set; }
        public List<int>? SelectedAnswers { get; set; }
        public List<int> CorrectAnswers { get; set; }

        
    }

}
