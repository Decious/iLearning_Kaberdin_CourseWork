using KaberdinCourseiLearning.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Data.ProductRequests
{
    public class ProductEditRequest : ProductRequest
    {
        [JsonPropertyName("productID")]
        public int ProductID { get; set; }
    }
}
