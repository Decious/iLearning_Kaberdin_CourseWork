using KaberdinCourseiLearning.Data.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Helpers
{
    public class UserValidator
    {
        private UserManager<IdentityUser> userManager;
        public UserValidator(UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
        }
        public async Task<bool> isUserValidAsync(IdentityUser user)
        {
            return user != null && !await userManager.IsLockedOutAsync(user);
        }

        public async Task<bool> isUserOwnerOrAdminAsync(IdentityUser user,string ownerName)
        {
            if(await isUserValidAsync(user))
            {
                return (user.UserName == ownerName || await userManager.IsInRoleAsync(user, RoleNames.ROLE_ADMINISTRATOR));
            }
            return false;
        }
    }
}
