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
        public ColumnType GetColumnType(int typeID) => context.ColumnTypes.Find(typeID);
        public string GetColumnTypeHtml(int typeID, string attributes = null, string inner = null)
        {
            var columnType = context.ColumnTypes.Find(typeID);
            return GetColumnTypeHtml(columnType, attributes, inner);
        }
        public string GetColumnTypeHtml(ColumnType type, string attributes = null, string inner = null)
        {
            if (type != null)
            {
                return ReplaceTemplate(type, attributes, inner);
            }
            return null;
        }
        private static string ReplaceTemplate(ColumnType type,string attributes=null,string inner=null)
        {
            var html = type.TypeHTML;
            var attr = attributes ?? "";
            var inn = inner ?? "";
            html = html.Replace("%attributes%", attr);
            html = html.Replace("%inner%", inn);
            return html;
        }
    }
}
