using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Platform_Learning_Test.Domain.Entities;

namespace Platform_Learning_Test.Domain.Extensions
{
    public static class QuestionDifficultyExtensions
    {
        public static string GetDisplayName(this QuestionDifficulty difficulty)
        {
            if (!Enum.IsDefined(typeof(QuestionDifficulty), difficulty))
                return difficulty.ToString();

            var field = difficulty.GetType().GetField(difficulty.ToString());
            return field?.GetCustomAttribute<DisplayAttribute>()?.Name ?? difficulty.ToString();
        }
    }


}
