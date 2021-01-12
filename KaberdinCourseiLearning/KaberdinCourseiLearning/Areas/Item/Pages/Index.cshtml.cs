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

        public IndexModel(ProductManager productManager)
        {
            this.productManager = productManager;
        }
        public Product Product { get; set; }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            Product = await productManager.GetProductWithReferencesAsync(id);
            ItemModel = new ItemModel()
            {
                Item = Product,
                PermittedToChange = true
            };
            return Page();
        }
        public ItemModel ItemModel { get; set; }
    }
}
