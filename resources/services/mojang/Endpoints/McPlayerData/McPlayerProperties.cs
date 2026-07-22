using System.Text.Json.Serialization;

namespace SovereigntyBot.Services.Endpoints.Mojang
{
    public class McPlayerProperties
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}