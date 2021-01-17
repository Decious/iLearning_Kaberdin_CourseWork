using KaberdinCourseiLearning.Data;
using KaberdinCourseiLearning.Data.Models;
using KaberdinCourseiLearning.Data.ProductRequests;
using KaberdinCourseiLearning.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Areas.Item.Pages
{
    [Authorize(PolicyNames.POLICY_AUTHENTICATED)]
    public class CreateModel : PageModel
    {
        private ProductManager productManager;
        private CustomUserManager userManager;
        private CollectionManager collectionManager;
        private TagManager tagManager;

        public CreateModel(CollectionManager collectionManager,ProductManager productManager,CustomUserManager userManager,TagManager tagManager)
        {
            this.productManager = productManager;
            this.userManager = userManager;
            this.collectionManager = collectionManager;
            this.tagManager = tagManager;
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
            if (productID != 0)
            {
                isEdit = true;
                Product = await productManager.GetProductWithReferencesAsync(productID);
                OwnerUserName = Product.Collection.User.UserName;
                Columns = Product.Collection.Columns;
                foreach(var columnValue in Product.ColumnValues)
                {
                    ColumnValues.Add(columnValue.ColumnID, columnValue.Value);
                }
            } else if (collectionID != 0)
            {
                isEdit = false;
                Collection = await collectionManager.GetCollectionAsyncWithReferences(collectionID);
                OwnerUserName = Collection.User.UserName;
                Columns = Collection.Columns;
                Product = new Product();
            }
            LoadTags();
            return Product != null || Collection != null;
        }
        private void LoadTags()
        {
            var tagObjects = tagManager.GetAllTags();
            var tagValues = new List<string>();
            foreach(var obj in tagObjects){
                tagValues.Add(obj.TagValue);
            }
            Tags = string.Join(",", tagValues);
        }
        public async Task<IActionResult> OnPostCreateProduct([FromBody] ProductCreateRequest request)
        {
            if (request == null) return new JsonResult(new ServerResponse(false, "Request invalid."));
            Collection = await collectionManager.GetCollectionAsyncWithReferences(request.CollectionID);
            if (Collection == null) return new JsonResult(new ServerResponse(false, "Collection no longer exists."));
            if (!await userManager.IsUserOwnerOrAdminAsync(User, Collection.User.UserName)) return new JsonResult(new ServerResponse(false, "You dont have permission to create products for this collection.", "/Collection?collectionID=" + request.CollectionID));
            
            var response = await productManager.CreateProductAsync(request);
            return new JsonResult(response);
        }
        public async Task<IActionResult> OnPostEditProduct([FromBody] ProductEditRequest request)
        {
            if (request == null) return new JsonResult(new ServerResponse(false, "Request invalid."));
            Product = await productManager.GetProductWithReferencesAsync(request.ProductID);
            if (Product == null) return new JsonResult(new ServerResponse(false, "Item no longer exists."));
            Collection = await collectionManager.GetCollectionAsyncWithReferences(Product.CollectionID);
            if (Collection == null) return new JsonResult(new ServerResponse(false, "Collection no longer exists."));
            if (!await userManager.IsUserOwnerOrAdminAsync(User, Collection.User.UserName)) return new JsonResult(new ServerResponse(false, "You dont have permission to edit this product.", "/Item?id=" + request.ProductID));
            
            var response = await productManager.EditProductAsync(request);
            return new JsonResult(response);
        }
    }
}
