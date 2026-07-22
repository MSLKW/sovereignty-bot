using System.Text.Json.Serialization;

namespace SovereigntyBot.Services.Endpoints.Hypixel
{
    public class Record
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }
        [JsonPropertyName("owner")]
        public string Owner { get; set; }
        [JsonPropertyName("limit")]
        public int Limit { get; set; }
        [JsonPropertyName("queriesInPastMin")]
        public int QueriesInPastMin { get; set; }
        [JsonPropertyName("totalQueries")]
        public int TotalQueries { get; set; }
    }
}