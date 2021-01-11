using KaberdinCourseiLearning.Data;
using KaberdinCourseiLearning.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Managers
{
    public class TagManager
    {
        private ApplicationDbContext context;
        public TagManager(ApplicationDbContext context)
        {
            this.context = context;
        }
        public Tag[] GetAllTags()=>context.Tags.ToArray();
        public Tag GetTag(int tagID) => context.Tags.Find(tagID);
        public Tag GetTag(string tagValue) => context.Tags.FirstOrDefault(e => e.TagValue == tagValue);
        public Product[] GetProductsWithTag(Tag tag)
        {
            context.Entry(tag).Collection(e => e.ProductTags);
            var products = new Product[tag.ProductTags.Count];
            foreach(var prodTag in tag.ProductTags)
            {
                products.Append(context.Products.Find(prodTag.ProductID));
            }
            return products.ToArray();
        }
        public async Task AddTagAsync(Tag tag)
        {
            await context.Tags.AddAsync(tag);
            await context.SaveChangesAsync();
        }
        public async Task UpdateTagAsync(Tag tag)
        {
            context.Tags.Update(tag);
            await context.SaveChangesAsync();
        }
    }
}
