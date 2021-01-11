using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Data.Models
{
    public class Tag
    {
        public Tag()
        {
            ProductTags = new List<ProductTag>();
        }
        [Key]
        public int TagID { get; set; }
        public string TagValue { get; set; }
        public ICollection<ProductTag> ProductTags { get; set; }
    }
}
