using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Data.Models
{
    public class ProductColumnValue
    {
        [ForeignKey("Product")]
        public int ProductID { get; set; }
        public Product Product { get; set; }
        [ForeignKey("Column")]
        public int ColumnID { get; set; }
        public ProductCollectionColumn Column { get; set; }
        public string Value { get; set; }
    }
}
