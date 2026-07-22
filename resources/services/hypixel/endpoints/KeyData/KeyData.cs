using System.Text.Json.Serialization;

namespace SovereigntyBot.Services.Endpoints.Hypixel
{
    public class KeyData : IHypixelSuccess
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }
        [JsonPropertyName("record")]
        public Record Record { get; set; }
    }
}