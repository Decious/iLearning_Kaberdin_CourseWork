using KaberdinCourseiLearning.Data;
using KaberdinCourseiLearning.Data.Models;
using System.Linq;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Managers
{
    public class CollectionManager
    {
        private ApplicationDbContext context;
        public CollectionManager(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<ProductCollection> GetCollectionAsync(int collectionID) => await context.ProductCollections.FindAsync(collectionID);
        public async Task LoadReferencesAsync(ProductCollection productCollection)
        {
            await context.Entry(productCollection).Collection(c => c.Products).LoadAsync();
            await context.Entry(productCollection).Collection(c => c.Columns).LoadAsync();
            await context.Entry(productCollection).Reference(c => c.User).LoadAsync();
        }
        public async Task DeleteCollectionAsync(int collectionID)
        {
            var coll = await GetCollectionAsync(collectionID);
            context.Remove(coll);
            await context.SaveChangesAsync();
        }
        public async Task EditCollectionAsync(ProductCollection newCollection)
        {
            context.Update(newCollection);
            await context.SaveChangesAsync();
        }
        public async Task CreateCollectionAsync(ProductCollection newCollection)
        {
            context.ProductCollections.Add(newCollection);
            await context.SaveChangesAsync();
        }
        public async Task AddCollectionColumnsAsync(ProductCollectionColumn[] columns)
        {
            context.ProductCollectionColumns.AddRange(columns);
            await context.SaveChangesAsync();
        }
        public ProductCollectionTheme[] GetCollectionThemes() => context.Themes.ToArray();
    }
}
