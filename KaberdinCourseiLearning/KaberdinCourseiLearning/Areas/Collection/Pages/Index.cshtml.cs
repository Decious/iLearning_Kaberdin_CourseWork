using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KaberdinCourseiLearning.Data;
using KaberdinCourseiLearning.Data.Models;
using KaberdinCourseiLearning.Helpers;
using KaberdinCourseiLearning.Managers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KaberdinCourseiLearning.Areas.Collection.Pages
{
    public class IndexModel : PageModel
    {
        private CollectionManager collectionManager;
        private CustomUserManager userManager;
        private ProductManager productManager;

        public IndexModel(CollectionManager collectionManager,ProductManager productManager,CustomUserManager userManager)
        {
            this.collectionManager = collectionManager;
            this.userManager = userManager;
            this.productManager = productManager;
        }
        public ProductCollection Collection { get; set; }
        public bool PermittedToChange { get; set; }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (await LoadPropertiesAsync(id)) return Page();
            return Redirect("~/Index");
        }
        private async Task<bool> LoadPropertiesAsync(int collectionID)
        {
            Collection = await collectionManager.GetCollectionAsync(collectionID);
            if (Collection == null) return false;
            await collectionManager.LoadReferencesAsync(Collection);
            foreach (var product in Collection.Products)
                await productManager.LoadReferencesAsync(product);
            PermittedToChange = await userManager.IsUserOwnerOrAdminAsync(User, Collection.User.UserName);
            return true;
        }
        public async Task<IActionResult> OnPostDeleteCollection(int collectionID)
        {
            var loaded = await LoadPropertiesAsync(collectionID);
            if (PermittedToChange && loaded)
            {
                await collectionManager.DeleteCollectionAsync(collectionID);
                return new JsonResult(ServerResponse.MakeSuccess());
            }
            return new JsonResult(ServerResponse.MakeForbidden());
        }
    }
}
