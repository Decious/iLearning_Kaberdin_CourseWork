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
        private string AvatarPath;
        private ApplicationDbContext context;
        public ProfileHelper(IWebHostEnvironment webHostEnvironment, ApplicationDbContext context)
        {
            AvatarPath = Path.Combine(webHostEnvironment.WebRootPath, "images", "User", "Avatar");
            this.context = context;
        }
        public async Task ChangeDescriptionAsync(string newText,string UserID)
        {
            var page = context.UserPages.FirstOrDefault(i => i.UserID == UserID);
            page.Description = newText;
            context.Update(page);
            await context.SaveChangesAsync();
        }
        public async Task UpdateAvatarAsync(IFormFile file,string UserID)
        {
            try
            {
                if (file.Length > 0)
                {
                    using (var stream = File.Create($"{AvatarPath}\\{UserID}.png"))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[AVATAR]Exception during file upload.\n {e.Message}");
            }
        }
    }
}
