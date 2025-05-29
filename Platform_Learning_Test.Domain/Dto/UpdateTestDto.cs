using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform_Learning_Test.Domain.Dto
{
    public class UpdateTestDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 3)]
        public string Title { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        public string Difficulty { get; set; }

        [Required]
        [StringLength(100)]
        public string Category { get; set; }
    }

}
