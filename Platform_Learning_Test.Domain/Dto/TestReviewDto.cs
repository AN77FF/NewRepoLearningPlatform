using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform_Learning_Test.Domain.Dto
{
    public class TestReviewDto
    {
        public int Id { get; set; }
        public int TestId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Text { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
