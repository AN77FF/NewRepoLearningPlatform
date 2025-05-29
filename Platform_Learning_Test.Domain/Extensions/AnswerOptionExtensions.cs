using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Platform_Learning_Test.Domain.Dto;
using Platform_Learning_Test.Domain.Entities;

namespace Platform_Learning_Test.Domain.Extensions
{
    public static class AnswerOptionExtensions
    {
        public static AnswerOptionDto ToDto(this AnswerOption answer)
        {
            if (answer == null) return null;

            return new AnswerOptionDto
            {
                Id = answer.Id,
                Text = answer.Text,
                IsCorrect = answer.IsCorrect,
                QuestionId = answer.QuestionId
            };
        }

        public static AnswerOption ToEntity(this CreateAnswerOptionDto dto)
        {
            if (dto == null) return null;

            return new AnswerOption
            {
                Text = dto.Text,
                IsCorrect = dto.IsCorrect
            };
        }

        public static AnswerOption ToEntity(this UpdateAnswerOptionDto dto)
        {
            if (dto == null) return null;

            return new AnswerOption
            {
                Id = dto.Id ?? 0,
                Text = dto.Text,
                IsCorrect = dto.IsCorrect
            };
        }
    }
    }
