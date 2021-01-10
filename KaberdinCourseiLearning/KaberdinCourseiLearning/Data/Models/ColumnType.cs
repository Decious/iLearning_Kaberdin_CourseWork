using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Data.Models
{
    public class ColumnType
    {
        [Key]
        public int TypeID { get; set; }
        public string TypeName { get; set; }
        public string TypeHTML { get; set; }
    }
}
