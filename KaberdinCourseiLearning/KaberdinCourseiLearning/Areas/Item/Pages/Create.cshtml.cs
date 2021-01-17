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
        private ProductManager productManager;
        private CustomUserManager userManager;
        private ApplicationDbContext context;

        public CreateModel(ApplicationDbContext context,ProductManager productManager,CustomUserManager userManager)
        {
            this.productManager = productManager;
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
                .Include(p => p.Tags).ThenInclude(t => t.Tag)
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
        public async Task<IActionResult> OnPostCreateProduct([FromBody] ProductCreateRequest request)
        {
            if (request == null) return new JsonResult(new ServerResponse(false, "Request invalid."));
            Collection = await context.ProductCollections.Where(c => c.CollectionID == request.CollectionID).Include(c => c.User).FirstOrDefaultAsync();
            if (Collection == null) return new JsonResult(new ServerResponse(false, "Collection no longer exists."));
            if (!await userManager.IsUserOwnerOrAdminAsync(User, Collection.User.UserName)) return new JsonResult(new ServerResponse(false, "You dont have permission to create products for this collection.", "/Collection?collectionID=" + request.CollectionID));
            
            var response = await productManager.CreateProductAsync(request);
            return new JsonResult(response);
        }
        public async Task<IActionResult> OnPostEditProduct([FromBody] ProductEditRequest request)
        {
            if (request == null) return new JsonResult(new ServerResponse(false, "Request invalid."));
            Product = await context.Products.FindAsync(request.ProductID);
            if (Product == null) return new JsonResult(new ServerResponse(false, "Item no longer exists."));
            Collection = await context.ProductCollections.Where(c => c.CollectionID == Product.CollectionID).Include(c => c.User).FirstOrDefaultAsync();
            if (Collection == null) return new JsonResult(new ServerResponse(false, "Collection no longer exists."));
            if (!await userManager.IsUserOwnerOrAdminAsync(User, Collection.User.UserName)) return new JsonResult(new ServerResponse(false, "You dont have permission to edit this product.", "/Item?id=" + request.ProductID));
            
            var response = await productManager.EditProductAsync(request);
            return new JsonResult(response);
        }
    }
}
