using KaberdinCourseiLearning.Data;
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
        private readonly UserManager<CustomUser> userManager;
        public UserValidator(UserManager<CustomUser> userManager)
        {
            this.userManager = userManager;
        }
        public async Task<bool> IsUserValidAsync(CustomUser user)
        {
            return user != null && !await userManager.IsLockedOutAsync(user);
        }

        public async Task<bool> IsUserOwnerOrAdminAsync(CustomUser user,string ownerName)
        {
            if(await IsUserValidAsync(user))
            {
                return (user.UserName == ownerName || await userManager.IsInRoleAsync(user, RoleNames.ROLE_ADMINISTRATOR));
            }
            return false;
        }
    }
}
