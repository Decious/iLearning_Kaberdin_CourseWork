using System.Text.Json.Serialization;

namespace KaberdinCourseiLearning.Data.ProductRequests
{
    public class ProductCreateRequest
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("tags")]
        public string Tags { get; set; }
        [JsonPropertyName("columnValues")]
        public string[] ColumnValues { get; set; }
        [JsonPropertyName("columnIDs")]
        public int[] ColumnIDs { get; set; }
    }
}
