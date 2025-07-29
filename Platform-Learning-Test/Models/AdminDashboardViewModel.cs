using Platform_Learning_Test.Domain.Dto;

namespace Platform_Learning_Test.Models
{
    public class AdminDashboardViewModel
    {
        public AdminDashboardStatsDto Stats { get; set; }
        public IEnumerable<TestDto> Tests { get; set; }
        public IEnumerable<QuestionDto> Questions { get; set; }
        public IEnumerable<AnswerOptionDto> Answers { get; set; }
    }
}
