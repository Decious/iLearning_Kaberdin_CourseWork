using KaberdinCourseiLearning.Data.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KaberdinCourseiLearning.Pages.Partials.Models
{
    public class CollectionModel : PageModel
    {
        public ProductCollection Collection { get; set; }
        public bool PermittedToChange { get; set; }
    }
}
