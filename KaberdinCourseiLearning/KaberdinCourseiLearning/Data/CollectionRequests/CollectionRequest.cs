using KaberdinCourseiLearning.Data.Models;
using KaberdinCourseiLearning.Resources;
using KaberdinCourseiLearning.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Data.CollectionRequests
{
    public class CollectionRequest
    {
        [JsonPropertyName("name")]
        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = nameof(ValidationResource.CollectionNameRequired))]
        [StringLength(50, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = nameof(ValidationResource.The_must_be_at_least_and_at_max_characters_long_), MinimumLength = 4)]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("theme")]
        public int ThemeID { get; set; }
        [JsonPropertyName("columns")]
        [ColumnsNameValidity(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = nameof(ValidationResource.ColumnNameRequired))]
        public ProductCollectionColumn[] Columns { get; set; }
    }
}
