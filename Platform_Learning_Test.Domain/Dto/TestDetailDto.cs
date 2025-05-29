using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform_Learning_Test.Domain.Dto
{
    public class TestDetailDto : TestDto
    {
        public List<QuestionDto> Questions { get; set; } = new();
        public int QuestionCount => Questions.Count;
    }

}
