using KaberdinCourseiLearning.Data.Models;
using KaberdinCourseiLearning.Managers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Middleware
{
    public class UserValidation
    {
        private RequestDelegate next;
        public UserValidation(RequestDelegate next)
        {
            this.next = next;
        }
        public async Task InvokeAsync(HttpContext context, UserManager<CustomUser> userManager, SignInManager<CustomUser> signInManager)
        {
            if (signInManager.IsSignedIn(context.User))
            {
                var user = await userManager.GetUserAsync(context.User);
                if (user == null || user.LockoutEnd > DateTime.Now)
                {
                    await signInManager.SignOutAsync();
                    context.Response.Redirect("/Identity/Account/Login");
                    return;
                }
            }
            await next.Invoke(context);
        }
    }
}
