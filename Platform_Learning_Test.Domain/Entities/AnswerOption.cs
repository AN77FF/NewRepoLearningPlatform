using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform_Learning_Test.Domain.Entities
{
    public class AnswerOption
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string Text { get; set; }

        public bool IsCorrect { get; set; }

        public int QuestionId { get; set; }
        public Question Question { get; set; }

        [MaxLength(1000)]
        public string Explanation { get; set; }
        public ICollection<UserAnswer> UserAnswers { get; set; }= new List<UserAnswer>();
    }

}
