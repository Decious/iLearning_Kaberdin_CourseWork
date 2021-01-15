using KaberdinCourseiLearning.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Data.CollectionRequests
{
    public class CollectionRequest
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("theme")]
        public int ThemeID { get; set; }
        [JsonPropertyName("columns")]
        public ProductCollectionColumn[] Columns { get; set; }
    }
}
