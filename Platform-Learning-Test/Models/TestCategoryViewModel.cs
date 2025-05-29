using Platform_Learning_Test.Domain.Dto;

namespace Platform_Learning_Test.Models
{
    public class TestCategoryViewModel
    {
        public List<string> Categories { get; set; }
        public IEnumerable<TestDto> Tests { get; set; }
    }
}
