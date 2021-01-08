using KaberdinCourseiLearning.Data;
using KaberdinCourseiLearning.Data.Models;
using KaberdinCourseiLearning.Managers;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Helpers
{
    public static class IdentityInitializer
    {
        public static void SeedData(CustomUserManager userManager,RoleManager<IdentityRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }
        public static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync(RoleNames.ROLE_ADMINISTRATOR).Result)
            {
                IdentityRole role = new IdentityRole(RoleNames.ROLE_ADMINISTRATOR);
                _ = roleManager.CreateAsync(role).Result;
            }
        }
        public static void SeedUsers(CustomUserManager userManager)
        {
            var users = userManager.GetUsersInRoleAsync(RoleNames.ROLE_ADMINISTRATOR).Result;
            if (users.Count < 1)
            {
                var defaultUser = new CustomUser("adminroot")
                {
                    Email = "iLearn@Secret.root"
                };
                var res = userManager.CreateAsync(defaultUser, "adminroot").Result;
                if (res.Succeeded)
                {
                    _ = userManager.AddToRoleAsync(defaultUser,RoleNames.ROLE_ADMINISTRATOR).Result;
                }
            }
        }
    }
}
