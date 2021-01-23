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
using Microsoft.EntityFrameworkCore;

namespace KaberdinCourseiLearning.Areas.Collection.Pages
{
    public class IndexModel : PageModel
    {
        private CustomUserManager userManager;
        private ApplicationDbContext context;

        public IndexModel(ApplicationDbContext context,CustomUserManager userManager)
        {
            this.userManager = userManager;
            this.context = context;
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
            Collection = await context.ProductCollections
                .Where(c => c.CollectionID == collectionID)
                .Include(c => c.User)
                .Include(c => c.Columns)
                .Include(c => c.Products).ThenInclude(p => p.ColumnValues)
                .AsSplitQuery().FirstOrDefaultAsync();
            if (Collection == null) return false;
            PermittedToChange = await userManager.IsUserOwnerOrAdminAsync(User, Collection.User.UserName);
            return true;
        }
    }
}
