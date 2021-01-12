using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KaberdinCourseiLearning.Data.Models;
using KaberdinCourseiLearning.Managers;
using KaberdinCourseiLearning.Pages.Partials.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KaberdinCourseiLearning.Areas.Item.Pages
{
    public class IndexModel : PageModel
    {
        private ProductManager productManager;
        private CustomUserManager userManager;

        public IndexModel(ProductManager productManager,CustomUserManager userManager)
        {
            this.productManager = productManager;
            this.userManager = userManager;
        }
        public Product Product { get; set; }
        public bool PermittedToChange { get; set; }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            Product = await productManager.GetProductWithReferencesAsync(id);
            if (Product == null) return Redirect("~/Index");
            PermittedToChange = await userManager.IsUserOwnerOrAdminAsync(User, Product.Collection.User.UserName);
            ItemModel = new ItemModel()
            {
                Item = Product,
                PermittedToChange = PermittedToChange
            };
            return Page();
        }
        public ItemModel ItemModel { get; set; }
    }
}
