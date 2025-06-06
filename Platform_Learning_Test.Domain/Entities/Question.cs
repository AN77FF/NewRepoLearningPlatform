using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Platform_Learning_Test.Domain.Entities
{
    public class Question
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Text { get; set; }

        public int TestId { get; set; }
        public Test Test { get; set; }

        public ICollection<AnswerOption> AnswerOptions { get; set; } = new List<AnswerOption>();
        public ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();

        public QuestionDifficulty Difficulty { get; set; } = QuestionDifficulty.Medium;

        public int TimeLimitSeconds { get; set; } = 60;
    }
    public enum QuestionDifficulty
    {
        Easy,
        Medium,
        Hard
    }

}
