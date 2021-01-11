using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Data.Models
{
    public class ProductTag
    {
        [ForeignKey("Tag")]
        public int TagID { get; set; }
        public Tag Tag { get; set; }
        [ForeignKey("Product")]
        public int ProductID { get; set; }
        public Product Product { get; set; }
    }
}
