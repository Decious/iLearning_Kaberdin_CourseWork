using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Middleware
{
    public static class UserValidationMiddlewareExtensions
    {
        public static IApplicationBuilder UseUserValidationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UserValidation>();
        }
    }
}
