using System;
using System.Collections.Generic;
using System.Linq;
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
                AnswerOptions = question.AnswerOptions?.Select(a =>
                    new AnswerOptionDto
                    {
                        Id = a.Id,
                        Text = a.Text,
                        IsCorrect = a.IsCorrect
                    }).ToList()
            };
        }
        public static UpdateQuestionDto ToUpdateDto(this Question question)
        {
            return new UpdateQuestionDto
            {
                Id = question.Id,
                Text = question.Text,
                Difficulty = question.Difficulty.ToString(),
                TimeLimitSeconds = question.TimeLimitSeconds,
                TestId = question.TestId,
                AnswerOptions = question.AnswerOptions.Select(a => new UpdateAnswerOptionDto
                {
                    Id = a.Id,
                    Text = a.Text,
                    IsCorrect = a.IsCorrect
                }).ToList()
            };
        }

        public static Question ToEntity(this CreateQuestionDto dto)
        {
            if (dto == null) return null;

            return new Question
            {
                Text = dto.Text,
                TestId = dto.TestId,
                Difficulty = QuestionDifficulty.Medium, 
                TimeLimitSeconds = 60, 
                AnswerOptions = dto.AnswerOptions.Select(a =>
                    new AnswerOption
                    {
                        Text = a.Text,
                        IsCorrect = a.IsCorrect
                    }).ToList()
            };
        }
    }
}