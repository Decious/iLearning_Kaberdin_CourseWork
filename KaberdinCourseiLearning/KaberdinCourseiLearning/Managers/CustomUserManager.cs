using KaberdinCourseiLearning.Data;
using KaberdinCourseiLearning.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Managers
{
    public class CustomUserManager : UserManager<CustomUser>
    {
        private ApplicationDbContext context;
        public CustomUserManager(ApplicationDbContext context,IUserStore<CustomUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<CustomUser> passwordHasher, IEnumerable<IUserValidator<CustomUser>> userValidators, IEnumerable<IPasswordValidator<CustomUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<CustomUser>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            this.context = context;
        }
        public async Task<CustomUser> FindUserByNameWithReferencesAsync(string name)
        {
            return await context.Users
                .Where(u => u.UserName == name)
                .Include(u => u.ItemCollections)
                .Include(u => u.HomePage)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> IsUserAdminAsync(CustomUser user)
        {
            return await IsInRoleAsync(user, RoleNames.ROLE_ADMINISTRATOR);
        }
        public async Task<bool> IsUserAdminAsync(ClaimsPrincipal principal)
        {
            var user = await GetUserAsync(principal);
            return await IsUserAdminAsync(user);
        }
        public async Task<bool> IsUserOwnerOrAdminAsync(CustomUser user, string ownerName)
        {
            if (user == null) return false;
            return (await IsUserAdminAsync(user) || user.UserName == ownerName);
        }
        public async Task<bool> IsUserOwnerOrAdminAsync(ClaimsPrincipal principal, string ownerName)
        {
            var user = await GetUserAsync(principal);
            return await IsUserOwnerOrAdminAsync(user, ownerName);
        }
    }
}
