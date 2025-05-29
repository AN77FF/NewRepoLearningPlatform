using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform_Learning_Test.Domain.Dto
{
    public class TestResultDetailsDto : TestResultDto
    {
        public List<QuestionResultDto> QuestionResults { get; set; } = new();

        public TimeSpan TimeSpent { get; set; }
    }

}
