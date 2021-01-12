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
    }
}
