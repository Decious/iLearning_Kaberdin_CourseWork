using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using KaberdinCourseiLearning.Areas.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using KaberdinCourseiLearning.Data.Models;

namespace KaberdinCourseiLearning.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<CustomUser> signInManager;
        private readonly ILogger<LogoutModel> logger;

        public LogoutModel(SignInManager<CustomUser> signInManager, ILogger<LogoutModel> logger)
        {
            this.signInManager = signInManager;
            this.logger = logger;
        }

        public async Task<IActionResult> OnGet()
        {
            await signInManager.SignOutAsync();
            logger.LogInformation("User logged out.");
            return Redirect("~/Index");
        }
    }
}
