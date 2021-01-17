using KaberdinCourseiLearning.Data;
using KaberdinCourseiLearning.Data.CollectionRequests;
using KaberdinCourseiLearning.Data.Models;
using Microsoft.EntityFrameworkCore;
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
        public async Task<ServerResponse> EditCollectionAsync(EditCollectionRequest request)
        {
            var collection = await context.ProductCollections
                             .Where(c => c.CollectionID == request.CollectionID)
                             .Include(c => c.Columns).FirstOrDefaultAsync();
            if (collection == null) return new ServerResponse(false, "No such collection found! Maybe collection got deleted?");
            var theme = await context.Themes.FindAsync(request.ThemeID);
            if (theme == null) return new ServerResponse(false, "No such theme found! Maybe theme got deleted?");
            DeleteColumns(request.DeletedColumns);
            UpdateCollection(collection, request, theme);
            UpdateColumns(collection, request);
            await context.SaveChangesAsync();
            return new ServerResponse(true, "Collection successfully updated!", "/Collection?id=" + collection.CollectionID);
        }
        private void DeleteColumns(int[] columnIDs)
        {
            foreach (var deletedColumnID in columnIDs)
            {
                var column = context.ProductCollectionColumns.Find(deletedColumnID);
                context.ProductCollectionColumns.Remove(column);
            }
        }
        private void UpdateCollection(ProductCollection collection,EditCollectionRequest request, ProductCollectionTheme theme)
        {
            collection.Description = request.Description;
            collection.Theme = theme.Theme;
            collection.Name = request.Name;
        }
        private void UpdateColumns(ProductCollection collection, EditCollectionRequest request)
        {
            foreach (var column in request.Columns)
            {
                if (column.ColumnID != 0)
                {
                    var tracked = context.ProductCollectionColumns.Find(column.ColumnID);
                    tracked.ColumnName = column.ColumnName;
                    tracked.TypeID = column.TypeID;
                    context.ProductCollectionColumns.Update(tracked);
                }
                else
                {
                    column.CollectionID = collection.CollectionID;
                    context.ProductCollectionColumns.Add(column);
                }
            }
        }
        public async Task<ServerResponse> CreateCollectionAsync(CreateCollectionRequest request)
        {
            var theme = await context.Themes.FindAsync(request.ThemeID);
            if (theme == null) return new ServerResponse(false, "No such theme found! Maybe theme got deleted?");
            var user = await context.Users.Where(u => u.UserName == request.PageUserName).FirstOrDefaultAsync();
            if (user == null) return new ServerResponse(false, "No such user found! Maybe user got deleted?");
            var id = await SaveCollection(request,theme,user.Id);
            return new ServerResponse(true, "Successfully created collection", "/Collection?id=" + id);
        }
        private async Task<int> SaveCollection(CreateCollectionRequest request, ProductCollectionTheme theme,string userID)
        {
            ProductCollection newColl = new ProductCollection() { Description = request.Description,UserID = userID, Theme = theme.Theme, Name = request.Name };
            context.ProductCollections.Add(newColl);
            await context.SaveChangesAsync();
            foreach (var column in request.Columns)
            {
                column.CollectionID = newColl.CollectionID;
            }
            await AddCollectionColumnsAsync(request.Columns);
            return newColl.CollectionID;
        }
        private async Task AddCollectionColumnsAsync(ProductCollectionColumn[] columns)
        {
            context.ProductCollectionColumns.AddRange(columns);
            await context.SaveChangesAsync();
        }
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
