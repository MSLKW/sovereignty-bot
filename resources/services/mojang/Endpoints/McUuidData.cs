using System.Text.Json.Serialization;

namespace SovereigntyBot.Services.Endpoints.Mojang
{
    public class McUuidData
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}