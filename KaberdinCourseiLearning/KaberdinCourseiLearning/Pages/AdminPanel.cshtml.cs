using KaberdinCourseiLearning.Areas.Identity;
using KaberdinCourseiLearning.Data;
using KaberdinCourseiLearning.Data.Models;
using KaberdinCourseiLearning.Helpers;
using KaberdinCourseiLearning.Managers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Pages
{
    [Authorize(Policy = PolicyNames.POLICY_ADMIN)]
    public class AdminPanelModel : PageModel
    {
        private readonly SignInManager<CustomUser> signInManager;
        private readonly CustomUserManager userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private string errorMessage;
        public AdminPanelModel(SignInManager<CustomUser> signInManager, CustomUserManager userManager,RoleManager<IdentityRole> roleManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        [BindProperty]
        public String FormAction { get; set; }
        [BindProperty]
        public String NewRole { get; set; }
        public CustomUser[] Users { get; set; }
        public IdentityRole[] Roles { get; set; }
        public IdentityUser CurrentUser { get; set; }

        public void OnGet()
        {
            PopulateProperties();
        }
        private void PopulateProperties()
        {
            PopulateUsers();
            PopulateRoles();
            CurrentUser = userManager.GetUserAsync(User).Result;
        }
        private void PopulateUsers()
        {
            var usersList = new List<CustomUser>();
            foreach (var user in userManager.Users)
            {
                usersList.Add(user);
            }
            Users = usersList.ToArray();
        }
        private void PopulateRoles()
        {
            var rolesList = new List<IdentityRole>();
            foreach (var role in roleManager.Roles)
            {
                rolesList.Add(role);
            }
            Roles = rolesList.ToArray();
        }
        private async Task SignOut()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            await signInManager.SignOutAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var resultIsSuccess = false;
            var valid = await IsUserAdmin();
            if (!valid) return Redirect("~/Index");
            var ids = Request.Form["Selected"];
            foreach (String id in ids)
            {
                var user = await userManager.FindByIdAsync(id);
                switch (FormAction)
                {
                    case PanelActions.ACTION_BLOCK:
                        resultIsSuccess = await SetLockout(user, DateTime.MaxValue);
                        break;
                    case PanelActions.ACTION_UNBLOCK:
                        if (await userManager.GetLockoutEndDateAsync(user) > DateTime.Now)
                        {
                            resultIsSuccess = await SetLockout(user, null);
                        }
                        break;
                    case PanelActions.ACTION_DELETE:
                            resultIsSuccess = HandleErrors(await userManager.DeleteAsync(user));
                        break;
                    case PanelActions.ACTION_ROLECHANGE:
                            resultIsSuccess = await ChangeRole(user);
                        break;
                }
            }
            valid = await IsUserAdmin();
            if (valid) return RedirectToPage(new { resultIsSuccess, errorMessage});
            return Redirect("~/Index");
        }
        private async Task<bool> SetLockout(CustomUser user, DateTime? time)
        {
            var res = await userManager.SetLockoutEndDateAsync(user, time);
            if (res.Succeeded)
            {
                var res2 = await userManager.UpdateAsync(user);
                if (!res2.Succeeded)
                {
                    return HandleErrors(res2);
                }
            } else
            {
                return HandleErrors(res);
            }
            return true;
        }
        private bool HandleErrors(IdentityResult result)
        {
            if (!result.Succeeded)
            {
                var errors = result.Errors;
                foreach (var error in errors)
                {
                    errorMessage += error.Code + "." + error.Description;
                }
            }
            return result.Succeeded;
        }
        private async Task<bool> ChangeRole(CustomUser user)
        {
            var currentRoles = await userManager.GetRolesAsync(user);
            if(HandleErrors(await userManager.RemoveFromRolesAsync(user, currentRoles)))
            {
                if (NewRole != "User")
                    return HandleErrors(await userManager.AddToRoleAsync(user, NewRole));
                return true;
            }
            return false;
        }
        private async Task<bool> IsUserAdmin()
        {
            var isValid = await userManager.IsUserOwnerOrAdminAsync(User, "");
            if (!isValid)
            {
                await SignOut();
            }
            return isValid;
        }

    }
}