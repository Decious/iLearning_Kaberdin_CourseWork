using KaberdinCourseiLearning.Data.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Pages.Partials.Models
{
    public class ItemModel : PageModel
    {
        public Product Item { get; set; }
        public bool PermittedToChange { get; set; }
    }
}
