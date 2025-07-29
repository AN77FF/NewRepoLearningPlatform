using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Platform_Learning_Test.Domain.Entities;

namespace Platform_Learning_Test.Domain.Dto
{
    public class UpdateQuestionDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(1000, MinimumLength = 5)]
        public string Text { get; set; }

        [Required]
        [EnumDataType(typeof(QuestionDifficulty))]
        public string Difficulty { get; set; }

        [Range(10, 300)]
        public int TimeLimitSeconds { get; set; }

        [Required]
        public int TestId { get; set; }



        [MinLength(2)]
        public List<UpdateAnswerOptionDto> AnswerOptions { get; set; } = new();
    }

}
