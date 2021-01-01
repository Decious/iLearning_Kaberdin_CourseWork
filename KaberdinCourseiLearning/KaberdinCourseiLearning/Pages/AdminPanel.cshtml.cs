using KaberdinCourseiLearning.Areas.Identity;
using KaberdinCourseiLearning.Data.Models;
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
        private SignInManager<IdentityUser> signInManager;
        private UserManager<IdentityUser> userManager;
        private RoleManager<IdentityRole> roleManager;
        public AdminPanelModel(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager,RoleManager<IdentityRole> roleManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        [BindProperty]
        public String FormAction { get; set; }
        [BindProperty]
        public String NewRole { get; set; }
        public IdentityUser[] Users { get; set; }
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
            var usersList = new List<IdentityUser>();
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
        private async Task<bool> isUserBannedOrRemovedAsync(IdentityUser user)
        {
            return (user == null || await userManager.IsLockedOutAsync(user));
        }
        private async Task SignOut()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            await signInManager.SignOutAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var valid = await isUserValid();
            if (!valid) return Redirect("~/Index");
            var ids = Request.Form["Selected"];
            foreach (String id in ids)
            {
                var user = await userManager.FindByIdAsync(id);
                switch (FormAction)
                {
                    case PanelActions.ACTION_BLOCK:
                        await SetLockout(user, DateTime.MaxValue);
                        break;
                    case PanelActions.ACTION_UNBLOCK:
                        if (await userManager.GetLockoutEndDateAsync(user) > DateTime.Now)
                        {
                            await SetLockout(user, null);
                        }
                        break;
                    case PanelActions.ACTION_DELETE:
                            await userManager.DeleteAsync(user);
                        break;
                    case PanelActions.ACTION_ROLECHANGE:
                            await ChangeRole(user);
                        break;
                }
            }
            valid = await isUserValid();
            if (valid) return RedirectToPage();
            return Redirect("~/Index");
        }
        private async Task SetLockout(IdentityUser user, DateTime? time)
        {
            await userManager.SetLockoutEndDateAsync(user, time);
            await userManager.UpdateAsync(user);
        }
        private async Task ChangeRole(IdentityUser user)
        {
            var currentRoles = await userManager.GetRolesAsync(user);
            await userManager.RemoveFromRolesAsync(user, currentRoles);
            if (NewRole != "User")
                await userManager.AddToRoleAsync(user, NewRole);
        }
        private async Task<bool> isUserValid()
        {
            var currentUser = await userManager.GetUserAsync(User);
            var isNotNullOrBanned = await isUserBannedOrRemovedAsync(currentUser);
            if (isNotNullOrBanned)
            {
                var currentRoles = await userManager.GetRolesAsync(currentUser);
                if (!currentRoles.Contains(RoleNames.ROLE_ADMINISTRATOR))
                {
                    return false;
                }
            } else
            {
                await SignOut();
                return false;
            }
            return true;
        }

    }
}