using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RozetkaParser
{
    public class ProductsData
    {
        [Key]
        public int Id { get; set; }

        [JsonProperty("data")]
        public List<Product> Data { get; set; }
    }
}
