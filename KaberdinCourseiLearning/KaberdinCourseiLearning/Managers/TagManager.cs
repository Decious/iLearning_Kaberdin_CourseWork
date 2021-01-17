using KaberdinCourseiLearning.Data;
using KaberdinCourseiLearning.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Managers
{
    public class TagManager
    {
        private ApplicationDbContext context;
        public TagManager(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<string> GetProductTags(Product prod)
        {
            await context.Entry(prod).Collection(c => c.Tags).LoadAsync();
            var prodtags = prod.Tags.ToArray();
            var tags = new List<string>();
            foreach(var tag in prodtags)
            {
                await context.Entry(tag).Reference(t => t.Tag).LoadAsync();
                tags.Add(tag.Tag.TagValue);
            }
            return string.Join(",", tags);
        }
    }
}
