using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Platform_Learning_Test.Domain.Dto;
using Platform_Learning_Test.Domain.Entities;

namespace Platform_Learning_Test.Domain.Extensions
{
    public static class TestExtensions
    {
        public static TestDto ToDto(this Test test)
        {
            if (test == null) return null;

            return new TestDto
            {
                Id = test.Id,
                Title = test.Title,
                Description = test.Description,
                Difficulty = test.Difficulty.GetDisplayName(),
                Category = test.Category,
                CreatedAt = test.CreatedAt
            };
        }

        public static TestDetailDto ToDetailDto(this Test test)
        {
            if (test == null) return null;

            return new TestDetailDto
            {
                Id = test.Id,
                Title = test.Title,
                Description = test.Description,
                Difficulty = test.Difficulty.GetDisplayName(),
                Category = test.Category,
                CreatedAt = test.CreatedAt,
                Questions = test.Questions?.Select(q => q.ToDto()).ToList()
            };
        }

        public static Test ToEntity(this CreateTestDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            if (!Enum.TryParse<TestDifficulty>(dto.Difficulty, true, out var difficulty))
            {
                throw new ValidationException($"Invalid difficulty value: {dto.Difficulty}");
            }

            return new Test
            {
                Title = dto.Title,
                Description = dto.Description,
                Difficulty = difficulty,
                Category = dto.Category
            };
        }

        public static Test ToEntity(this UpdateTestDto dto, Test existingTest)
        {

            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "DTO cannot be null");

            if (existingTest == null)
                throw new ArgumentNullException(nameof(existingTest), "Existing test cannot be null");

            if (!Enum.TryParse<TestDifficulty>(dto.Difficulty, ignoreCase: true, out var difficulty))
            {
                var validValues = string.Join(", ", Enum.GetNames(typeof(TestDifficulty)));
                throw new ValidationException(
                    $"Invalid difficulty value: {dto.Difficulty}. Valid values are: {validValues}");
            }


            if (!string.Equals(existingTest.Title, dto.Title, StringComparison.Ordinal))
                existingTest.Title = dto.Title;

            if (!string.Equals(existingTest.Description, dto.Description, StringComparison.Ordinal))
                existingTest.Description = dto.Description;

            if (existingTest.Difficulty != difficulty)
                existingTest.Difficulty = difficulty;

            if (!string.Equals(existingTest.Category, dto.Category, StringComparison.Ordinal))
                existingTest.Category = dto.Category;


            existingTest.UpdatedAt = DateTime.UtcNow;

            return existingTest;
        }
    }
}
