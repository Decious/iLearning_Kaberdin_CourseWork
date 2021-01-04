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
        private UserManager<CustomUser> userManager;
        public UserValidator(UserManager<CustomUser> userManager)
        {
            this.userManager = userManager;
        }
        public async Task<bool> isUserValidAsync(CustomUser user)
        {
            return user != null && !await userManager.IsLockedOutAsync(user);
        }

        public async Task<bool> isUserOwnerOrAdminAsync(CustomUser user,string ownerName)
        {
            if(await isUserValidAsync(user))
            {
                return (user.UserName == ownerName || await userManager.IsInRoleAsync(user, RoleNames.ROLE_ADMINISTRATOR));
            }
            return false;
        }
    }
}
