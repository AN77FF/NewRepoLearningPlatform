using Platform_Learning_Test.Domain.Dto;

namespace Platform_Learning_Test.Models
{
    public class TestEditViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<QuestionDto> Questions { get; set; }
    }
}
