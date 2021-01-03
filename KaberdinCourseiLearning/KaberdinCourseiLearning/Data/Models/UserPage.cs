using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Data.Models
{
    public class UserPage
    {
        [Key]
        public int PageID { get; set; }
        [ForeignKey("User")]
        public string UserID { get; set; }
        public CustomUser User { get; set; }
        public string? Description { get; set; }
    }
}
