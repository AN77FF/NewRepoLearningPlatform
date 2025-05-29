using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform_Learning_Test.Domain.Dto
{
    public class UpdateAnswerOptionDto
    {
        public int? Id { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 1)]
        public string Text { get; set; }

        public bool IsCorrect { get; set; }

        public int QuestionId { get; set; }
    }

}
