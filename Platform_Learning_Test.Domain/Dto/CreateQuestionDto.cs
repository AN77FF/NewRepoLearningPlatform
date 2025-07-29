using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Platform_Learning_Test.Domain.Dto;
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

        public List<AnswerOptionDto> AnswerOptions { get; set; } = new(); 

    //    [MinLength(2, ErrorMessage = "Должно быть не менее 2 вариантов ответа")]
    //    public List<CreateAnswerOptionDto> AnswerOptions { get; set; } = new List<CreateAnswerOptionDto>
    //{
    //    new CreateAnswerOptionDto(),
    //    new CreateAnswerOptionDto() 
    //};
    }
}
