using KaberdinCourseiLearning.Data;
using KaberdinCourseiLearning.Data.Models;
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
        public async Task DeleteProductAsync(int productID)
        {
            var pr = await GetProductAsync(productID);
            context.Remove(pr);
            await context.SaveChangesAsync();
        }
        public async Task EditCollectionAsync(Product newProduct)
        {
            context.Update(newProduct);
            await context.SaveChangesAsync();
        }
        public async Task CreateCollectionAsync(Product newProduct)
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
