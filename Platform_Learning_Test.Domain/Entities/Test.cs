using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform_Learning_Test.Domain.Entities
{
    public class Test
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [Required]
        public TestDifficulty Difficulty { get; set; }

        [Required]
        [MaxLength(100)]
        public string Category { get; set; }

        public ICollection<Question> Questions { get; set; } = new List<Question>();
    }

    public enum TestDifficulty
    {
        [Display(Name = "Легкий")]
        Easy,

        [Display(Name = "Средний")]
        Medium,

        [Display(Name = "Сложный")]
        Hard
    }


}
