using KaberdinCourseiLearning.Data;
using KaberdinCourseiLearning.Data.Models;
using KaberdinCourseiLearning.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Pages
{
    [Authorize(Policy = PolicyNames.POLICY_ADMIN)]
    public class AdminPanelModel : PageModel
    {
        private readonly CustomUserManager userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IStringLocalizer<AdminPanelModel> localizer;
        private ServerResponse notAuthorizedResult;
        private ServerResponse successResult;
        private ServerResponse userNotFoundResult;
        public AdminPanelModel(CustomUserManager userManager,RoleManager<IdentityRole> roleManager,IStringLocalizer<AdminPanelModel> localizer)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.localizer = localizer;
            notAuthorizedResult = new ServerResponse(false, localizer["Forbidden"]);
            successResult = new ServerResponse(true, localizer["Successful"]);
            userNotFoundResult = new ServerResponse(false, localizer["NotFoundError"]);
        }

        public CustomUser[] Users { get; set; }
        public IdentityRole[] Roles { get; set; }
        public CustomUser CurrentUser { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!await userManager.IsUserAdminAsync(User)) return Forbid();
            await PopulatePropertiesAsync();
            return Page();
        }
        private async Task PopulatePropertiesAsync()
        {
            Users = userManager.Users.ToArray();
            Roles = roleManager.Roles.ToArray();
            CurrentUser = await userManager.GetUserAsync(User);
        }
        public async Task<IActionResult> OnPostBlock(string[] ids)
        {
            return await DoAction(ids, BlockUser);
        }
        private async Task<ServerResponse> BlockUser(string id) => await SetLockout(id, DateTime.MaxValue);
        public async Task<IActionResult> OnPostUnblock(string[] ids)
        {
            return await DoAction(ids, UnblockUser);
        }
        private async Task<ServerResponse> UnblockUser(string id) => await SetLockout(id, null);
        private async Task<ServerResponse> SetLockout(string id, DateTime? time)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                await userManager.SetLockoutEndDateAsync(user, time);
                return successResult;
            }
            return userNotFoundResult;
        }
        public async Task<IActionResult> OnPostDelete(string[] ids)
        {
            return await DoAction(ids, DeleteUser);
        }
        private async Task<ServerResponse> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                await userManager.DeleteAsync(user);
                return successResult;
            }
            return userNotFoundResult;
        }
        private async Task<JsonResult> DoAction(string[] ids, Func<string, Task<ServerResponse>> Action)
        {
            if (!await userManager.IsUserAdminAsync(User))
                return new JsonResult(notAuthorizedResult);
            foreach (var id in ids)
            {
                var result = await Action(id);
                if (!result.Successful) return new JsonResult(result);
            }
            return new JsonResult(successResult);
        }
        public async Task<IActionResult> OnPostRolechange(string id, string newRole)
        {
            if (!await userManager.IsUserAdminAsync(User))
                return new JsonResult(notAuthorizedResult);
            return await ChangeRole(id, newRole);
        }
        private async Task<JsonResult> ChangeRole(string id, string newRole)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                await RemoveFromCurrentRoles(user);
                if (RoleNames.ROLE_USER != newRole && await roleManager.RoleExistsAsync(newRole))
                    await userManager.AddToRoleAsync(user, newRole);
                return new JsonResult(new ServerResponse(true, localizer["RoleChange", user.UserName, newRole]));
            }
            return new JsonResult(userNotFoundResult);
        }
        private async Task RemoveFromCurrentRoles(CustomUser user)
        {
            var currentRoles = await userManager.GetRolesAsync(user);
            await userManager.RemoveFromRolesAsync(user, currentRoles);
        }
    }
}