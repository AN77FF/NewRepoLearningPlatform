using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform_Learning_Test.Domain.Dto
{
    public class CreateTestReviewDto
    {
        [Required]
        public int TestId { get; set; }

        [Required]
        [StringLength(2000, MinimumLength = 10)]
        public string Text { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; } = 5;
    }

}
