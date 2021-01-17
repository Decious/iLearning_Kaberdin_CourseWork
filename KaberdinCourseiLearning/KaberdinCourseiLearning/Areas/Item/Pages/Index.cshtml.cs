using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KaberdinCourseiLearning.Data;
using KaberdinCourseiLearning.Data.Models;
using KaberdinCourseiLearning.Managers;
using KaberdinCourseiLearning.Pages.Partials.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace KaberdinCourseiLearning.Areas.Item.Pages
{
    public class IndexModel : PageModel
    {
        private ApplicationDbContext context;
        private ProductManager productManager;
        private CustomUserManager userManager;

        public IndexModel(ApplicationDbContext context,ProductManager productManager,CustomUserManager userManager)
        {
            this.context = context;
            this.productManager = productManager;
            this.userManager = userManager;
        }
        public Product Product { get; set; }
        public CustomUser GuestUser { get; set; }
        public bool PermittedToChange { get; set; }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            Product = await context.Products
                .Where(p => p.ProductID == id)
                .Include(p => p.Comments)
                .Include(p => p.Likes)
                .Include(p => p.Collection).ThenInclude(c=>c.User)
                .Include(p => p.Tags).ThenInclude(p => p.Tag)
                .Include(p => p.ColumnValues).ThenInclude(c => c.Column)
                .AsSplitQuery().FirstOrDefaultAsync();
            if (Product == null) return Redirect("~/Index");
            GuestUser = await userManager.GetUserAsync(User);
            PermittedToChange = await userManager.IsUserOwnerOrAdminAsync(User, Product.Collection.User.UserName);
            ItemModel = new ItemModel()
            {
                Item = Product,
                PermittedToChange = PermittedToChange
            };
            return Page();
        }
        public async Task<IActionResult> OnPostDeleteProduct(int id)
        {
            Product = await context.Products
                .Where(p => p.ProductID == id)
                .Include(p => p.Collection)
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync();
            if (Product == null) return new JsonResult(new ServerResponse(false,"Product doesnt exist","/"));
            PermittedToChange = await userManager.IsUserOwnerOrAdminAsync(User, Product.Collection.User.UserName);
            if (PermittedToChange)
            {
                var coll_ID = Product.CollectionID;
                await productManager.DeleteProductAsync(id);
                return new JsonResult(new ServerResponse(true,"Product successfully deleted.","/Collection?id="+ coll_ID));
            }
            return new JsonResult(ServerResponse.MakeForbidden());
        }
        public ItemModel ItemModel { get; set; }
    }
}
