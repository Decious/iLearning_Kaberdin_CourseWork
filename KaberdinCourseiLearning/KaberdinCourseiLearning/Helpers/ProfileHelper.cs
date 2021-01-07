using KaberdinCourseiLearning.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Helpers
{
    public class ProfileHelper
    {
        private ApplicationDbContext context;
        public ProfileHelper(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task ChangeDescriptionAsync(string newText,string UserID)
        {
            var page = context.UserPages.FirstOrDefault(i => i.UserID == UserID);
            page.Description = newText;
            context.Update(page);
            await context.SaveChangesAsync();
        }
    }
}
