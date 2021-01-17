using KaberdinCourseiLearning.Data;
using KaberdinCourseiLearning.Data.Models;
using KaberdinCourseiLearning.Data.ProductRequests;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Managers
{
    public class ProductManager
    {
        private ApplicationDbContext context;
        public ProductManager(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<Product> GetProductAsync(int productID) => await context.Products.FindAsync(productID);
        public async Task DeleteProductAsync(int productID)
        {
            var pr = await GetProductAsync(productID);
            context.Remove(pr);
            await context.SaveChangesAsync();
        }
        public async Task<ServerResponse> EditProductAsync(ProductEditRequest request)
        {
            var product = context.Products.Find(request.ProductID);
            if (product == null) return new ServerResponse(false, "Item does not exist.");
            await UpdateColumnValuesAsync(request.ColumnValues);
            await RemoveProductTagsAsync(request.ProductID);
            await AddProductTagsAsync(request.Tags,request.ProductID);
            product.Name = request.Name;
            await context.SaveChangesAsync();
            return new ServerResponse(true, "Item successfully updated!", "/Item?id=" + product.ProductID);
        }
        private async Task UpdateColumnValuesAsync(IEnumerable<ProductColumnValue> columnValues)
        {
            foreach (var columnValue in columnValues)
            {
                if (columnValue.ProductID != 0)
                {
                    var tracked = await context.ProductColumnValues.FindAsync(columnValue.ProductID, columnValue.ColumnID);
                    if(tracked != null)
                    {
                        tracked.Value = columnValue.Value;
                        context.Update(tracked);
                    }
                }
            }
            await context.SaveChangesAsync();
        }
        private async Task RemoveProductTagsAsync(int productID)
        {
            var tags = await context.ProductTags.Where(t => t.ProductID == productID).ToArrayAsync();
            context.RemoveRange(tags);
            await context.SaveChangesAsync();
        }
        public async Task<ServerResponse> CreateProductAsync(ProductCreateRequest request)
        {
            var newProduct = new Product() { Name = request.Name, CollectionID = request.CollectionID };
            await context.AddAsync(newProduct);
            await context.SaveChangesAsync();
            await addColumnValuesAsync(request.ColumnValues, newProduct.ProductID);
            await AddProductTagsAsync(request.Tags, newProduct.ProductID);
            return new ServerResponse(true, "Successfully created product.", "/Item?id=" + newProduct.ProductID);
        }
        private async Task addColumnValuesAsync(IEnumerable<ProductColumnValue> columnValues,int productID)
        {
            foreach(var columnValue in columnValues)
            {
                if (columnValue.ProductID == 0)
                {
                    columnValue.ProductID = productID;
                }
            }
            await context.ProductColumnValues.AddRangeAsync(columnValues);
            await context.SaveChangesAsync();
        }
        private async Task AddProductTagsAsync(string[] tags, int productID)
        {
            foreach (var tag in tags)
            {
                var found = context.Tags.FirstOrDefault(e => e.TagValue == tag);
                if (found == null)
                {
                    found = new Tag() { TagValue = tag };
                    await context.Tags.AddAsync(found);
                    await context.SaveChangesAsync();
                }
                var newPrTag = new ProductTag() { ProductID = productID, TagID = found.TagID };
                await context.ProductTags.AddAsync(newPrTag);
                await context.SaveChangesAsync();
            }
        }
        public async Task AddComment(Comment comm)
        {
            context.Comments.Add(comm);
            await context.SaveChangesAsync();
        }
        public async Task<Product[]> FindProducts(string query)
        {
            if (query == null) return null;
            return await context.Products.Where(p =>
                        p.SearchVector.Matches(query) ||
                        p.Collection.SearchVector.Matches(query) ||
                        p.Comments.Any(c => c.SearchVector.Matches(query)) ||
                        p.ColumnValues.Any(cv => cv.SearchVector.Matches(query)) ||
                        p.Tags.Any(t => t.Tag.SearchVector.Matches(query))
                        ).ToArrayAsync();
        }
        public async Task<Product[]> FindProductsByTag(string tag)
        {
            if (tag == null) return null;
            return await context.Products.Where(p =>
                            p.Tags.Any(t => t.Tag.SearchVector.Matches(tag))
                            ).ToArrayAsync();
        }
        public async Task<Product[]> FindProductsByOwner(string owner)
        {
            if (owner == null) return null;
            return await context.Products.Where(p => 
            p.Collection.User.UserName == owner
            ).ToArrayAsync();
        }
    }
}
