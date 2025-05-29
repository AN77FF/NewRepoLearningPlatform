using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Platform_Learning_Test.Domain.Dto;
using Platform_Learning_Test.Domain.Entities;

namespace Platform_Learning_Test.Domain.Extensions
{
    public static class QuestionExtensions
    {
        public static QuestionDto ToDto(this Question question)
        {
            if (question == null) return null;

            return new QuestionDto
            {
                Id = question.Id,
                Text = question.Text,
                TestId = question.TestId,
                Difficulty = question.Difficulty.ToString(),
                TimeLimitSeconds = question.TimeLimitSeconds,
                AnswerOptions = question.AnswerOptions?.Select(a => a.ToDto()).ToList()
            };
        }

        public static Question ToEntity(this CreateQuestionDto dto)
        {
            if (dto == null) return null;

            return new Question
            {
                Text = dto.Text,
                TestId = dto.TestId,
                Difficulty = Enum.Parse<QuestionDifficulty>(dto.Difficulty),
                TimeLimitSeconds = dto.TimeLimitSeconds,
                AnswerOptions = dto.AnswerOptions?.Select(a => a.ToEntity()).ToList()
            };
        }

        public static Question ToEntity(this UpdateQuestionDto dto, Question existingQuestion)
        {
            if (dto == null || existingQuestion == null)
                return existingQuestion;

            existingQuestion.Text = dto.Text;
            existingQuestion.Difficulty = Enum.Parse<QuestionDifficulty>(dto.Difficulty);
            existingQuestion.TimeLimitSeconds = dto.TimeLimitSeconds;

            return existingQuestion;
        }
    }


}
