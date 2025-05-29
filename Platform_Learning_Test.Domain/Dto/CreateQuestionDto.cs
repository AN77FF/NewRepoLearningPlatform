using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Platform_Learning_Test.Domain.Entities;

namespace Platform_Learning_Test.Domain.Dto
{
    public class CreateQuestionDto
    {
        [Required]
        [StringLength(1000, MinimumLength = 5)]
        public string Text { get; set; }

        [Required]
        public int TestId { get; set; }

        [Required]
        [EnumDataType(typeof(QuestionDifficulty))]
        public string Difficulty { get; set; } = "Medium";

        [Range(10, 300)]
        public int TimeLimitSeconds { get; set; } = 60;

        [MinLength(2)]
        public List<CreateAnswerOptionDto> AnswerOptions { get; set; } = new();
    }
}
