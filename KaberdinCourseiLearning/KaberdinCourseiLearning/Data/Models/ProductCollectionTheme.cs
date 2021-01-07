using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Data.Models
{
    public class ProductCollectionTheme
    {
        [Key]
        public int ThemeID { get; set; }
        public string Theme { get; set; }
    }
}
