using System.Text.Json.Serialization;

namespace KaberdinCourseiLearning.Data.CollectionRequests
{
    public class EditCollectionRequest : CollectionRequest
    {
        [JsonPropertyName("collectionID")]
        public int CollectionID { get; set; }
        [JsonPropertyName("deletedColumns")]
        public int[] DeletedColumns { get; set; }
    }
}
