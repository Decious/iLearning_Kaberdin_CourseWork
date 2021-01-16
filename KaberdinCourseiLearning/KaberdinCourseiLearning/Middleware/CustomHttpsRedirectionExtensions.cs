using Microsoft.AspNetCore.Builder;

namespace KaberdinCourseiLearning.Middleware
{
    public static class CustomHttpsRedirectionExtensions
    {
        public static IApplicationBuilder UseCustomHttpsRedirection(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomHttpsRedirection>();
        }
    }
}
