using System.Text.Json.Serialization;

namespace SovereigntyBot.Services.Endpoints.Hypixel
{
    public class GuildData : IHypixelSuccess
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }
        [JsonPropertyName("guild")]
        public Guild Guild { get; set; }
    }
}