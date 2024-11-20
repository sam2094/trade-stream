using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class BinanceResponse
    {
        [JsonPropertyName("s")]
        public string Currency { get; set; } 

        [JsonPropertyName("p")]
        public string Price { get; set; }
    }
}