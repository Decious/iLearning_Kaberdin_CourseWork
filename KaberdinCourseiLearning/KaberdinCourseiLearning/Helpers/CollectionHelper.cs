using KaberdinCourseiLearning.Data;
using KaberdinCourseiLearning.Data.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Helpers
{
    public class CollectionHelper
    {
        private string BGPath;
        private ApplicationDbContext context;
        public CollectionHelper(IWebHostEnvironment webHostEnvironment, ApplicationDbContext context)
        {
            BGPath = Path.Combine(webHostEnvironment.WebRootPath, "images", "Collection", "Background");
            this.context = context;
        }
        public async Task DeleteCollectionAsync(int collectionID)
        {
            var coll = context.ProductCollections.FirstOrDefault(i => i.CollectionID == collectionID);
            context.Remove(coll);
            await context.SaveChangesAsync();
            if (System.IO.File.Exists($"{BGPath}\\{collectionID}.png"))
                System.IO.File.Delete($"{BGPath}\\{collectionID}.png");
        }
        public async Task EditCollectionAsync(int collectionID,ProductCollection newCollection)
        {
            var coll = context.ProductCollections.FirstOrDefault(i => i.CollectionID == collectionID);
            context.Update(newCollection);
            await context.SaveChangesAsync();
        }
        public async Task<int> CreateCollectionAsync(ProductCollection newCollection)
        {
            context.ProductCollections.Add(newCollection);
            await context.SaveChangesAsync();
            return newCollection.CollectionID;
        }
        public async Task AddCollectionColumnsAsync(ProductCollectionColumn[] columns)
        {
            context.ProductCollectionColumns.AddRange(columns);
            await context.SaveChangesAsync();
        }
        public async Task UpdateBackgroundAsync(int collectionID,IFormFile file)
        {
            try
            {
                if (file.Length > 0)
                {
                    using (var stream = File.Create($"{BGPath}\\{collectionID}.png"))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[COLLECTION]Exception during file upload.\n {e.Message}");
            }
        }
    }
}
