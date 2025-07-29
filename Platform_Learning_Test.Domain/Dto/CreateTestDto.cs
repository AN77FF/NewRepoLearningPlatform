using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Platform_Learning_Test.Domain.Entities;

namespace Platform_Learning_Test.Domain.Dto
{
    public class CreateTestDto
    {
        [Required]
        [StringLength(200, MinimumLength = 3)]
        public string Title { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        [EnumDataType(typeof(TestDifficulty))]
        public string Difficulty { get; set; }

        [Required]
        [StringLength(100)]
        public string Category { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public TimeSpan Duration { get; set; } = TimeSpan.FromHours(1);
        public bool IsFeatured { get; set; } = false;

    }

}
