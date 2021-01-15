using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Data.CollectionRequests
{
    public class CreateCollectionRequest : CollectionRequest
    {
        [JsonPropertyName("pageUserName")]
        public string PageUserName { get; set; }
    }
}
