using KaberdinCourseiLearning.Data;
using KaberdinCourseiLearning.Data.Models;
using System.Linq;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Helpers
{
    public class CollectionHelper
    {
        private ApplicationDbContext context;
        public CollectionHelper(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task DeleteCollectionAsync(int collectionID)
        {
            var coll = context.ProductCollections.FirstOrDefault(i => i.CollectionID == collectionID);
            context.Remove(coll);
            await context.SaveChangesAsync();
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
    }
}
