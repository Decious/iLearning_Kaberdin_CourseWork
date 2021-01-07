using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Data.Models
{
    public class Tag
    {
        [Key]
        public int TagID { get; set; }
        public string TagValue { get; set; }
    }
}
