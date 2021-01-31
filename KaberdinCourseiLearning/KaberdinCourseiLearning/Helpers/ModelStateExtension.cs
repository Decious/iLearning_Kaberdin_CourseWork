using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Helpers
{
    public static class ModelStateExtension
    {
        public static string[] GetModelErrors(this ModelStateDictionary modelState)
        {
            var totalErrors = new List<string>();
            foreach (var state in modelState.Values)
            {
                foreach (var error in state.Errors)
                {
                    totalErrors.Add(error.ErrorMessage);
                }
            }
            return totalErrors.ToArray();
        }
    }
}
