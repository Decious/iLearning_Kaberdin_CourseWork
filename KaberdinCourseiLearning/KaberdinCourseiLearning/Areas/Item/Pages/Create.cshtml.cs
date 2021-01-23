using KaberdinCourseiLearning.Data;
using KaberdinCourseiLearning.Data.Models;
using KaberdinCourseiLearning.Data.ProductRequests;
using KaberdinCourseiLearning.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Areas.Item.Pages
{
    [Authorize(PolicyNames.POLICY_AUTHENTICATED)]
    public class CreateModel : PageModel
    {
        private CustomUserManager userManager;
        private ApplicationDbContext context;

        public CreateModel(ApplicationDbContext context,CustomUserManager userManager)
        {
            this.userManager = userManager;
            this.context = context;
        }
        public ProductCollection Collection { get; set; }
        public IEnumerable<ProductCollectionColumn> Columns { get; set; }
        public Product Product { get; set; }
        public string OwnerUserName { get; set; }
        public Dictionary<int,string> ColumnValues { get; set; }
        public bool isEdit { get; set; }
        public string Tags { get; set; }
        public async Task<IActionResult> OnGetAsync(int collectionID,int productID)
        {
            if(await TryLoadPropertiesAsync(collectionID,productID))
            {
                if (await userManager.IsUserOwnerOrAdminAsync(User, OwnerUserName))
                {
                    return Page();
                }
                return Forbid();
            }
            return Redirect("~/Index");
        }
        private async Task<bool> TryLoadPropertiesAsync(int collectionID, int productID)
        {
            ColumnValues = new Dictionary<int, string>();
            isEdit = productID != 0;
            if (isEdit)
                await LoadProductAsync(productID);
            else
                await LoadCollectionAsync(collectionID);
            Columns = Collection.Columns;
            OwnerUserName = Collection.User.UserName;
            LoadTags();
            return Product != null || Collection != null;
        }
        private async Task LoadProductAsync(int productID)
        {
            Product = await context.Products
                .Where(p => p.ProductID == productID)
                .Include(p => p.Collection).ThenInclude(c => c.User)
                .Include(p => p.Collection).ThenInclude(c => c.Columns)
                .Include(p => p.ColumnValues)
                .AsSplitQuery().FirstOrDefaultAsync();
            Collection = Product.Collection;
            foreach (var columnValue in Product.ColumnValues)
            {
                ColumnValues.Add(columnValue.ColumnID, columnValue.Value);
            }
        }
        private async Task LoadCollectionAsync(int collectionID)
        {
            Collection = await  context.ProductCollections
                .Where(c => c.CollectionID == collectionID)
                .Include(c => c.User)
                .Include(c => c.Columns)
                .FirstOrDefaultAsync();
            Product = new Product();
        }
        private void LoadTags()
        {
            var tagObjects = context.Tags.ToArray();
            var tagValues = new List<string>();
            foreach(var obj in tagObjects){
                tagValues.Add(obj.TagValue);
            }
            Tags = string.Join(",", tagValues);
        }
    }
}
