using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Data.Models
{
    public class ProductCollection
    {
        [Key]
        public int CollectionID { get; set; }
        [ForeignKey("User")]
        public string UserID { get; set; }
        public CustomUser User { get; set; }
        public string Name { get; set; }
        public string Theme { get; set; }
        public string? Description { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
