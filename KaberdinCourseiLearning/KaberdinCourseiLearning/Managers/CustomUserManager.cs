using KaberdinCourseiLearning.Data;
using KaberdinCourseiLearning.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Managers
{
    public class CustomUserManager : UserManager<CustomUser>
    {
        public CustomUserManager(IUserStore<CustomUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<CustomUser> passwordHasher, IEnumerable<IUserValidator<CustomUser>> userValidators, IEnumerable<IPasswordValidator<CustomUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<CustomUser>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
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
            return (await IsUserAdminAsync(user) || user.UserName == ownerName);
        }
        public async Task<bool> IsUserOwnerOrAdminAsync(ClaimsPrincipal principal, string ownerName)
        {
            var user = await GetUserAsync(principal);
            return await IsUserOwnerOrAdminAsync(user, ownerName);
        }
    }
}
