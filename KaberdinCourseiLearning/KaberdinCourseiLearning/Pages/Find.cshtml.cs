using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KaberdinCourseiLearning.Data;
using KaberdinCourseiLearning.Data.Models;
using KaberdinCourseiLearning.Managers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;

namespace KaberdinCourseiLearning.Pages
{
    public class FindModel : PageModel
    {
        private ProductManager productManager;

        public FindModel(ProductManager productManager)
        {
            this.productManager = productManager;
        }
        public async Task<IActionResult> OnGetAsync(string q)
        {
            if(q != null)
            {
                SearchedFor = q;
                if (q.StartsWith('#'))
                {
                    q = q.Remove(0, 1);
                    MatchedResult = await productManager.FindProductsByTag(q);
                }else if (q.StartsWith('@'))
                {
                    q = q.Remove(0, 1);
                    MatchedResult = await productManager.FindProductsByOwner(q);
                }
                else
                {
                    MatchedResult = await productManager.FindProducts(q);
                }
            }
            return Page();
        }
        public Product[] MatchedResult { get; set; }
        public string SearchedFor { get; set; }
    }
}
