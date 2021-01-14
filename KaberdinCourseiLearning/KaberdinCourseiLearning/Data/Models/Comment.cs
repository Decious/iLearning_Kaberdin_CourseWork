using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Data.Models
{
    public class Comment
    {
        public Comment()
        {
            CreationTime = DateTime.Now;
        }
        [Key]
        public int CommentID { get; set; }
        [ForeignKey("Product")]
        public int ProductID { get; set; }
        public Product Product { get; set; }
        [ForeignKey("User")]
        public string UserID { get; set; }
        public CustomUser User { get; set; }
        public string Message { get; set; }
        public DateTime CreationTime { get; set; }
        public NpgsqlTsVector SearchVector { get; set; }
    }
}
