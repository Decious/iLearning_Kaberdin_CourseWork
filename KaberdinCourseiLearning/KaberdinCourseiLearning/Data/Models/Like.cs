using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Data.Models
{
    public class Like
    {
        [ForeignKey("User")]
        public String UserID { get; set; }
        public CustomUser User { get; set; }
        [ForeignKey("Product")]
        public int ProductID { get; set; }
        public Product Product { get; set; }
    }
}
