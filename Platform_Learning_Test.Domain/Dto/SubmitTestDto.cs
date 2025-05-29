using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform_Learning_Test.Domain.Dto
{
    public class SubmitTestDto
    {
        public int TestId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }
        public List<UserAnswerDto> Answers { get; set; } = new();
    }

}
