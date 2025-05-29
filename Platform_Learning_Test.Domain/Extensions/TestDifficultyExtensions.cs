using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Platform_Learning_Test.Domain.Entities;
using System.Reflection;
namespace Platform_Learning_Test.Domain.Extensions
{
    public static class TestDifficultyExtensions
    {
        public static string GetDisplayName(this TestDifficulty difficulty)
        {
            var fieldInfo = difficulty.GetType().GetField(difficulty.ToString());
            return fieldInfo?.GetCustomAttribute<DisplayAttribute>()?.Name ?? difficulty.ToString();
        }
    }

}
