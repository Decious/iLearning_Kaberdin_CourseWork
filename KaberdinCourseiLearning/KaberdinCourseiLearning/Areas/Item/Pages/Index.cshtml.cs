using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KaberdinCourseiLearning.Data;
using KaberdinCourseiLearning.Data.Models;
using KaberdinCourseiLearning.Managers;
using KaberdinCourseiLearning.Pages.Partials.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace KaberdinCourseiLearning.Areas.Item.Pages
{
    public class IndexModel : PageModel
    {
        private ApplicationDbContext context;
        private CustomUserManager userManager;

        public IndexModel(ApplicationDbContext context,CustomUserManager userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }
        public Product Product { get; set; }
        public CustomUser GuestUser { get; set; }
        public bool PermittedToChange { get; set; }
        public ItemModel ItemModel { get; set; }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            Product = await context.Products
                .Where(p => p.ProductID == id)
                .Include(p => p.Comments).ThenInclude(c=>c.User)
                .Include(p => p.Likes)
                .Include(p => p.Collection).ThenInclude(c=>c.User)
                .Include(p => p.ColumnValues).ThenInclude(c => c.Column)
                .AsSplitQuery().FirstOrDefaultAsync();
            if (Product == null) return Redirect("~/Index");
            GuestUser = await userManager.GetUserAsync(User);
            PermittedToChange = await userManager.IsUserOwnerOrAdminAsync(User, Product.Collection.User.UserName);
            var liked = false;
            if (GuestUser != null)
            {
                liked = Product.Likes.Where(l => l.UserID == GuestUser.Id).Any();
            }
            ItemModel = new ItemModel()
            {
                Item = Product,
                PermittedToChange = PermittedToChange,
                isLiked = liked
            };
            return Page();
        }
    }
}
