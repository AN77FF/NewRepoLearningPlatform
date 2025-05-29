using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Platform_Learning_Test.Domain.Dto;

namespace Platform_Learning_Test.Service.Service
{
    public interface IQuestionService
    {

        Task<QuestionDto> GetQuestionAsync(int id);
        Task<IEnumerable<QuestionDto>> GetQuestionsForTestAsync(int testId);
        Task<QuestionDto> CreateQuestionAsync(CreateQuestionDto dto);
        Task UpdateQuestionAsync(UpdateQuestionDto dto);
        Task DeleteQuestionAsync(int id);
        Task ValidateQuestionAnswersAsync(int questionId, List<int> selectedAnswerIds);
    }

}
