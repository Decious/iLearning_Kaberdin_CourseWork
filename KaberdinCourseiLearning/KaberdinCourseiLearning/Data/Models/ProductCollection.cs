using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KaberdinCourseiLearning.Data.Models
{
    public class ProductCollection
    {
        public ProductCollection()
        {
            Products = new List<Product>();
            CreationDate = DateTime.UtcNow;
            BackgroundUrl = "https://res.cloudinary.com/ilearningcourse/image/upload/v1610032560/Collection/default.jpg";
        }
        [Key]
        public int CollectionID { get; set; }
        [ForeignKey("User")]
        public string UserID { get; set; }
        public CustomUser User { get; set; }
        public string Name { get; set; }
        public string Theme { get; set; }
        public string? Description { get; set; }
        public DateTime CreationDate { get; set; }
        public ICollection<Product> Products { get; set; }
        public ICollection<ProductCollectionColumn> Columns { get; set; }
        public string BackgroundUrl { get; set; }
        public NpgsqlTsVector SearchVector { get; set; }
    }
}
