﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Data.Models
{
    public class Product
    {
        public Product()
        {
            Likes = new List<Like>();
            Comments = new List<Comment>();
            CreationDate = DateTime.UtcNow;
        }
        [Key]
        public int ProductID { get; set; }
        public string Name { get; set; }
        public string? Tags { get; set; }
        public DateTime CreationDate { get; set; }
        public ICollection<Like> Likes { get; set; }
        public ICollection<Comment> Comments { get; set; }
        [ForeignKey("Collection")]
        public int CollectionID { get; set; }
        public ProductCollection Collection { get; set; }
    }
}