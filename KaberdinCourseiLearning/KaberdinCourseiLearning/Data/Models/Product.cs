using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KaberdinCourseiLearning.Data.Models
{
    public class Product
    {
        public Product()
        {
            Likes = new List<Like>();
            Comments = new List<Comment>();
            ColumnValues = new List<ProductColumnValue>();
            CreationDate = DateTime.UtcNow;
            Tags = "";
        }
        [Key]
        public int ProductID { get; set; }
        public string? Name { get; set; }
        public string? Tags { get; set; }
        public DateTime CreationDate { get; set; }
        public ICollection<Like> Likes { get; set; }
        public ICollection<ProductColumnValue> ColumnValues { get; set; }
        public ICollection<Comment> Comments { get; set; }
        [ForeignKey("Collection")]
        public int CollectionID { get; set; }
        public ProductCollection Collection { get; set; }
        public NpgsqlTsVector SearchVector { get; set; }
    }
}
