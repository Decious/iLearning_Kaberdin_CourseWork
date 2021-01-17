using KaberdinCourseiLearning.Data;
using KaberdinCourseiLearning.Data.Models;
using KaberdinCourseiLearning.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
        private JsonResult notAuthorizedResult = new JsonResult(ServerResponse.MakeForbidden());
        private JsonResult successResult = new JsonResult(ServerResponse.MakeSuccess());
        public AdminPanelModel(CustomUserManager userManager,RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
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
        private async Task BlockUser(string id) => await SetLockout(id, DateTime.MaxValue);
        public async Task<IActionResult> OnPostUnblock(string[] ids)
        {
            return await DoAction(ids, UnblockUser);
        }
        private async Task UnblockUser(string id) => await SetLockout(id, null);
        private async Task SetLockout(string id, DateTime? time)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                await userManager.SetLockoutEndDateAsync(user, time);
            }
        }
        public async Task<IActionResult> OnPostDelete(string[] ids)
        {
            return await DoAction(ids, DeleteUser);
        }
        private async Task DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user != null)
                await userManager.DeleteAsync(user);
        }
        private async Task<JsonResult> DoAction(string[] ids, Func<string, Task> Action)
        {
            if (!await userManager.IsUserAdminAsync(User))
                return notAuthorizedResult;
            foreach (var id in ids)
            {
                await Action(id);
            }
            return successResult;
        }
        public async Task<IActionResult> OnPostRolechange(string id, string newRole)
        {
            if (!await userManager.IsUserAdminAsync(User))
                return notAuthorizedResult;
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
                return new JsonResult(new ServerResponse(true, $"{user.UserName} role was changed to {newRole}!"));
            }
            return new JsonResult(new ServerResponse(false, "User was not found! Try reloading the page."));
        }
        private async Task RemoveFromCurrentRoles(CustomUser user)
        {
            var currentRoles = await userManager.GetRolesAsync(user);
            await userManager.RemoveFromRolesAsync(user, currentRoles);
        }
    }
}