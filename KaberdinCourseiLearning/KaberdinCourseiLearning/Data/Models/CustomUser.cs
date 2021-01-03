using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Data.Models
{
    public class CustomUser : IdentityUser
    {
        public CustomUser() : base()
        {
        }
        public CustomUser(string userName) : base(userName)
        {
        }

        public UserPage HomePage { get; set; }
        public ICollection<ProductCollection> ItemCollections { get; set; }
    }
}
