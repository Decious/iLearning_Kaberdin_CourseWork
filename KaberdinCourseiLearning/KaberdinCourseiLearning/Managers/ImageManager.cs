using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using KaberdinCourseiLearning.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace KaberdinCourseiLearning.Managers
{
    public class ImageManager
    {
        private string cloudinaryURL;
        private Cloudinary cloudinary;
        private ApplicationDbContext context;
        public ImageManager(IConfiguration configuration, ApplicationDbContext context)
        {
            cloudinaryURL = configuration.GetConnectionString("CLOUDINARY_URL");
            cloudinary = new Cloudinary(cloudinaryURL);
            this.context = context;
        }
        public async Task<string> UploadAvatar(IFormFile file,string userID)
        {
            var folder = "Avatar/";
            var url = await UploadImage(file, userID, folder);
            context.Users.Find(userID).AvatarUrl = url;
            await context.SaveChangesAsync();
            return url;
        }
        public async Task<string> UploadBackground(IFormFile file, int collectionID)
        {
            var folder = "Collection/";
            var url = await UploadImage(file, collectionID.ToString(), folder);
            context.ProductCollections.Find(collectionID).BackgroundUrl = url;
            await context.SaveChangesAsync();
            return url;
        }
        private async Task<string> UploadImage(IFormFile file, string objectID,string folder)
        {
            var parameters = new ImageUploadParams()
            {
                File = new FileDescription(objectID,file.OpenReadStream()),
                UseFilename=true,
                Folder = folder
            };
            var result = await cloudinary.UploadAsync(parameters);
            return result.Url.ToString();
        }
    }
}
