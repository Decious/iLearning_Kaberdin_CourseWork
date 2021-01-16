using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Middleware
{
    public class CustomHttpsRedirection
    {
        private const string ForwardedProtoHeader = "X-Forwarded-Proto";
        private readonly RequestDelegate _next;

        public CustomHttpsRedirection(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext ctx)
        {
            var h = ctx.Request.Headers;
            if (h[ForwardedProtoHeader] == string.Empty || h[ForwardedProtoHeader] == "https")
            {
                await _next(ctx);
            }
            else if (h[ForwardedProtoHeader] != "https")
            {
                var withHttps = $"https://{ctx.Request.Host}{ctx.Request.Path}{ctx.Request.QueryString}";
                ctx.Response.Redirect(withHttps);
            }
        }
    }
}
