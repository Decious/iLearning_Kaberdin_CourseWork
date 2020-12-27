using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Areas.Identity
{
    public enum PermissionLevel
    {
        USER,
        ADMIN
    }
    public class CustomIdentity : IdentityUser
    {
        public PermissionLevel Permission { get; set; }
    }
}
