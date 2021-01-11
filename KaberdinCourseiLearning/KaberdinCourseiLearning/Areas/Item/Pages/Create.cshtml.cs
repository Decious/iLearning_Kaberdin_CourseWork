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
        public string Tags { get; set; }
        public async Task<IActionResult> OnGetAsync(int collectionID)
        {
            if(await TryLoadPropertiesAsync(collectionID))
            {
                if (await userManager.IsUserOwnerOrAdminAsync(User, Collection.User.UserName))
                {
                    return Page();
                }
                return Forbid();
            }
            return Redirect("~/Index");
        }
        private async Task<bool> TryLoadPropertiesAsync(int CollectionID)
        {
            Collection = await collectionManager.GetCollectionAsync(CollectionID);
            if (Collection != null)
            {
                await collectionManager.LoadReferencesAsync(Collection);
                LoadTags();
                return true;
            }
            return false;
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
        public async Task<IActionResult> OnPostAsync([FromBody] ProductCreateRequest createRequest,int collectionID)
        {
            if(isInputCorrect(createRequest, collectionID))
            {
                Collection = await collectionManager.GetCollectionAsync(collectionID);
                if (Collection == null) return new JsonResult(new ServerResponse(false,"Error while creating product. Collection no longer exists."));
                if (!await userManager.IsUserOwnerOrAdminAsync(User, Collection.User.UserName)) return new JsonResult(ServerResponse.MakeForbidden());
                var id = await CreateProduct(createRequest, collectionID);
                return new JsonResult(new ServerResponse(true, "Successfully created product.", "/Item?id=" + id));
            }
            return new JsonResult(new ServerResponse(false,"Error while creating product. Field name is empty."));
        }
        public bool isInputCorrect(ProductCreateRequest createRequest, int collectionID)
        {
            return createRequest != null && collectionID != 0 && !string.IsNullOrWhiteSpace(createRequest.Name);
        }

        public async Task<int> CreateProduct(ProductCreateRequest createRequest, int collectionID)
        {
            var newProduct = new Product() { Name = createRequest.Name, CollectionID = collectionID };
            await productManager.CreateProductAsync(newProduct);
            var newColumnValues = new ProductColumnValue[createRequest.ColumnValues.Length];
            for (int i = 0; i < createRequest.ColumnValues.Length; i++)
            {
                newColumnValues[i] = new ProductColumnValue() { ProductID = newProduct.ProductID, Value = createRequest.ColumnValues[i], ColumnID = createRequest.ColumnIDs[i] };
            }
            await productManager.AddProductColumnValuesAsync(newColumnValues);
            var tags = createRequest.Tags.Split(",");
            await tagManager.AddProductTags(tags, newProduct.ProductID);
            return newProduct.ProductID;
        }
    }
}
