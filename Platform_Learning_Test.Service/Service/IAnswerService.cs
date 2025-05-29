using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Platform_Learning_Test.Domain.Dto;

namespace Platform_Learning_Test.Service.Service
{
    public interface IAnswerService
    {
        Task<AnswerOptionDto> GetAnswerAsync(int id);
        Task<IEnumerable<AnswerOptionDto>> GetAnswersForQuestionAsync(int questionId);
        Task<AnswerOptionDto> CreateAnswerAsync(CreateAnswerOptionDto dto);
        Task UpdateAnswerAsync(UpdateAnswerOptionDto dto);
        Task DeleteAnswerAsync(int id);
        Task<bool> IsAnswerCorrectAsync(int answerId);
    }


}
