﻿using KaberdinCourseiLearning.Data;
using KaberdinCourseiLearning.Data.Models;
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
        public async Task AddProductTagAsync(ProductTag tag)
        {
            await context.ProductTags.AddAsync(tag);
            await context.SaveChangesAsync();
        }
        public async Task<string> GetProductTags(Product prod)
        {
            await context.Entry(prod).Collection(c => c.Tags).LoadAsync();
            var prodtags = prod.Tags.ToArray();
            var tags = new List<string>();
            foreach(var tag in prodtags)
            {
                await context.Entry(tag).Reference(t => t.Tag).LoadAsync();
                tags.Add(tag.Tag.TagValue);
            }
            return string.Join(",", tags);
        }
    }
}
