﻿using Microsoft.AspNetCore.Identity;
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
            HomePage = new UserPage() { Description = "I'm a default user!" };
            ItemCollections = new List<ProductCollection>();
            AvatarUrl = "https://res.cloudinary.com/ilearningcourse/image/upload/v1610032591/Avatar/default.webp";
        }
        public CustomUser(string userName) : base(userName)
        {
            HomePage = new UserPage() { Description = "I'm a default user!" };
            ItemCollections = new List<ProductCollection>();
        }

        public string? AvatarUrl { get; set; }
        public UserPage HomePage { get; set; }
        public ICollection<ProductCollection> ItemCollections { get; set; }
    }
}
