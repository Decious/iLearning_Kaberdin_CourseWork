using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using KaberdinCourseiLearning.Data;
using KaberdinCourseiLearning.Data.CollectionRequests;
using KaberdinCourseiLearning.Data.Models;
using KaberdinCourseiLearning.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace KaberdinCourseiLearning.Areas.Collection.Pages
{
    [Authorize(PolicyNames.POLICY_AUTHENTICATED)]
    public class CreateModel : PageModel
    {
        private ApplicationDbContext context;
        private CustomUserManager userManager;
        public CreateModel(CustomUserManager userManager,ApplicationDbContext context)
        {
            this.context = context;
            this.userManager = userManager;
        }
        public CustomUser PageUser { get; set; }
        public bool isEdit { get; set; }
        public ProductCollection Collection { get; set; }
        public ProductCollectionTheme[] Themes { get; set; }
        public ColumnType[] Types { get; set; }

        public async Task<IActionResult> OnGetAsync(string name,int id)
        {
            if (name != null || id != 0)
            {
                if (await TryLoadPropertiesAsync(name,id))
                {
                    if (!await userManager.IsUserOwnerOrAdminAsync(User, PageUser.UserName)) return Forbid();
                    return Page();
                }
            }
            return Redirect("~/Index");
        }
        private async Task<bool> TryLoadPropertiesAsync(string name,int id)
        {
            if (id != 0)
            {
                Collection = await context.ProductCollections
                    .Where(c => c.CollectionID == id)
                    .Include(c => c.Columns)
                    .Include(c=>c.User)
                    .FirstOrDefaultAsync();
                PageUser = Collection?.User;
                isEdit = true;
            }
            else if (name != null)
            {
                PageUser = await userManager.FindByNameAsync(name);
                isEdit = false;
            }
            Types = context.ColumnTypes.ToArray();
            Themes = context.Themes.ToArray();
            return PageUser != null || Collection != null;
        }
    }
}
