using KaberdinCourseiLearning.Data;
using KaberdinCourseiLearning.Data.CollectionRequests;
using KaberdinCourseiLearning.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Managers
{
    public class CollectionManager
    {
        private ApplicationDbContext context;
        private IStringLocalizer<CollectionManager> localizer;

        public CollectionManager(ApplicationDbContext context,IStringLocalizer<CollectionManager> localizer)
        {
            this.context = context;
            this.localizer = localizer;
        }
        public async Task<ServerResponse> EditCollectionAsync(EditCollectionRequest request)
        {
            var collection = await context.ProductCollections
                             .Where(c => c.CollectionID == request.CollectionID)
                             .Include(c => c.Columns).FirstOrDefaultAsync();
            if (collection == null) return new ServerResponse(false, localizer["NoCollectionError"]);
            var theme = await context.Themes.FindAsync(request.ThemeID);
            if (theme == null) return new ServerResponse(false, localizer["NoThemeError"]);
            UpdateCollection(collection, request, theme);
            UpdateColumns(collection, request);
            var newColumns = GetNewColumns(collection);
            await context.SaveChangesAsync();
            if (newColumns.Count() > 0)
            {
                SetDefaultColumnValues(collection, newColumns);
                await context.SaveChangesAsync();
            }
            return new ServerResponse(true, localizer["UpdateSuccess"], "/Collection?id=" + collection.CollectionID);
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
                    tracked.AllowedValues = column.AllowedValues;
                    context.ProductCollectionColumns.Update(tracked);
                }
                else
                {
                    column.CollectionID = collection.CollectionID;
                    context.ProductCollectionColumns.Add(column);
                }
            }
            DeleteColumns(collection);
        }
        private void DeleteColumns(ProductCollection collection)
        {
            foreach (var column in collection.Columns)
            {
                if (context.Entry(column).State == EntityState.Unchanged)
                    context.ProductCollectionColumns.Remove(column);
            }
        }
        private IEnumerable<ProductCollectionColumn> GetNewColumns(ProductCollection collection)
        {
            var addedColumns = new List<ProductCollectionColumn>();
            foreach (var column in collection.Columns)
            {
                if (context.Entry(column).State == EntityState.Added)
                {
                    addedColumns.Add(column);
                }
            }
            return addedColumns;
        }
        private void SetDefaultColumnValues(ProductCollection collection, IEnumerable<ProductCollectionColumn> addedColumns)
        {
            var affectedProducts = context.Products.Where(p => p.CollectionID == collection.CollectionID).Include(p => p.ColumnValues).ToArray();
            foreach(var product in affectedProducts)
            {
                foreach(var newColumn in addedColumns)
                {
                    product.ColumnValues.Add(new ProductColumnValue { ColumnID = newColumn.ColumnID, ProductID = product.ProductID, Value = "" });
                }
            }
        }
        public async Task<ServerResponse> CreateCollectionAsync(CreateCollectionRequest request)
        {
            var theme = await context.Themes.FindAsync(request.ThemeID);
            if (theme == null) return new ServerResponse(false, localizer["NoThemeError"]);
            var user = await context.Users.Where(u => u.UserName == request.PageUserName).FirstOrDefaultAsync();
            if (user == null) return new ServerResponse(false, localizer["NoUserError"]);
            var id = await SaveCollection(request,theme,user.Id);
            return new ServerResponse(true, localizer["CreateSuccess"], "/Collection?id=" + id);
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
