using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Platform_Learning_Test.Domain.Entities
{
    public class TestResult
    {
        public int Id { get; set; }
        public int TestId { get; set; }
        public string UserId { get; set; }
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
        public DateTime CompletionDate { get; set; }

        public Test Test { get; set; }
    }
}
