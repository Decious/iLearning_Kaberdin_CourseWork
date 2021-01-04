using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Data.Models
{
    public class ProductCollectionColumn
    {
        [Key]
        public int ColumnID { get; set; }
        [ForeignKey("Collection")]
        public int CollectionID { get; set; }
        public ProductCollection Collection { get; set; }
        public string ColumnType { get; set; }
        public string ColumnName { get; set; }
    }
}
