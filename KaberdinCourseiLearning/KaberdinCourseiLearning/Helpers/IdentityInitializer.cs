using KaberdinCourseiLearning.Data.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Helpers
{
    public static class IdentityInitializer
    {
        public static void SeedData(UserManager<CustomUser> userManager,RoleManager<IdentityRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }
        public static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync(RoleNames.ROLE_ADMINISTRATOR).Result)
            {
                IdentityRole role = new IdentityRole(RoleNames.ROLE_ADMINISTRATOR);
                var r = roleManager.CreateAsync(role).Result;
            }
        }
        public static void SeedUsers(UserManager<CustomUser> userManager)
        {
            var users = userManager.GetUsersInRoleAsync(RoleNames.ROLE_ADMINISTRATOR).Result;
            if (users.Count < 1)
            {
                var defaultUser = new CustomUser("adminroot");
                defaultUser.Email = "iLearn@Secret.root";
                var res = userManager.CreateAsync(defaultUser, "adminroot").Result;
                if (res.Succeeded)
                {
                    var r = userManager.AddToRoleAsync(defaultUser,RoleNames.ROLE_ADMINISTRATOR).Result;
                }
            }
        }
    }
}
