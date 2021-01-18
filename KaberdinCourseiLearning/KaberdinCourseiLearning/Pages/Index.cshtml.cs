using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KaberdinCourseiLearning.Data;
using KaberdinCourseiLearning.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KaberdinCourseiLearning.Pages
{
    public class IndexModel : PageModel
    {
        private ApplicationDbContext context;

        public IndexModel(ApplicationDbContext context)
        {
            this.context = context;
        }
        public IEnumerable<Tag> Tags { get; set; }
        public IEnumerable<Product> LastItems { get; set; }
        public IEnumerable<ProductCollection> BiggestCollections { get; set; }
        public void OnGet()
        {
            Tags = context.Tags.ToArray();
            LastItems = context.Products.Include(p=>p.Collection).Take(10).OrderByDescending(p => p.CreationDate).ToArray();
            BiggestCollections = context.ProductCollections.Take(10).OrderByDescending(c => c.Products.Count).ToArray();
        }
    }
}
