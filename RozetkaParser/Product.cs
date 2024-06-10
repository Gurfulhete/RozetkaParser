using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;


namespace RozetkaParser
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [JsonProperty("id")]
        public long ProductId { get; set; }
    
        [JsonProperty("title")]
        public string Title { get; set; }
    
        [JsonProperty("price")]
        public long Price { get; set; }
    
        [JsonProperty("old_price")]
        public long OldPrice { get; set; }
    
        [JsonProperty("price_pcs")]
        public string PricePcs { get; set; }
    
        [JsonProperty("min_month_price")]
        public long MinMonthPrice { get; set; }
    
        [JsonProperty("href")]
        public Uri Href { get; set; }
    
        [JsonProperty("status")]
        public string Status { get; set; }
    
        [JsonProperty("status_inherited")]
        public string StatusInherited { get; set; }
    
        [JsonProperty("sell_status")]
        public string SellStatus { get; set; }
    
        [JsonProperty("category_id")]
        public long CategoryId { get; set; }
    
        [JsonProperty("seller_id")]
        public long SellerId { get; set; }
    
        [JsonProperty("merchant_id")]
        public long MerchantId { get; set; }
    }
}
