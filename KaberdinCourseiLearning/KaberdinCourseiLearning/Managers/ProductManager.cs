using KaberdinCourseiLearning.Data;
using KaberdinCourseiLearning.Data.Models;
using Microsoft.EntityFrameworkCore;
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
        public async Task LoadReferencesAsync(Product product)
        {
            await context.Entry(product).Collection(c => c.Comments).LoadAsync();
            await context.Entry(product).Collection(c => c.Likes).LoadAsync();
            await context.Entry(product).Collection(c => c.ColumnValues).LoadAsync();
            await context.Entry(product).Reference(c => c.Collection).LoadAsync();
        }
        public async Task<Product> GetProductWithReferencesAsync(int productID)
        {
            return await context.Products.Where(p => p.ProductID == productID)
                .Include(p => p.Comments)
                .Include(p => p.Likes)
                .Include(p => p.Collection)
                .ThenInclude(c=>c.User)
                .Include(p => p.Tags)
                .ThenInclude(t => t.Tag)
                .Include(p => p.ColumnValues)
                .ThenInclude(c => c.Column)
                .ThenInclude(c => c.Type)
                .AsSplitQuery()
                .FirstOrDefaultAsync();
        }
        public async Task DeleteProductAsync(int productID)
        {
            var pr = await GetProductAsync(productID);
            context.Remove(pr);
            await context.SaveChangesAsync();
        }
        public async Task EditProductAsync(Product newProduct)
        {
            context.Update(newProduct);
            await context.SaveChangesAsync();
        }
        public async Task CreateProductAsync(Product newProduct)
        {
            context.Products.Add(newProduct);
            await context.SaveChangesAsync();
        }
        public async Task AddProductColumnValuesAsync(ProductColumnValue[] columnValues)
        {
            context.ProductColumnValues.AddRange(columnValues);
            await context.SaveChangesAsync();
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
