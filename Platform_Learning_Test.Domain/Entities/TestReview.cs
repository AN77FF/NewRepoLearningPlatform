using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform_Learning_Test.Domain.Entities
{
    public class TestReview
    {
        public int Id { get; set; }

        [Required]
        [StringLength(2000)]
        public string Text { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int UserId { get; set; }
        public User User { get; set; }

        public int TestId { get; set; }
        public Test Test { get; set; }
    }

}
