using System.Text.Json.Serialization;

namespace KaberdinCourseiLearning.Data.ProductRequests
{
    public class ProductCreateRequest : ProductRequest
    {
        [JsonPropertyName("collectionID")]
        public int CollectionID { get; set; }
    }
}
