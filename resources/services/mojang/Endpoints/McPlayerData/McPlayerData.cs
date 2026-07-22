using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace SovereigntyBot.Services.Endpoints.Mojang
{
    public class McPlayerData
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("properties")]
        public List<McPlayerProperties> Properties { get; set; }
    }
}