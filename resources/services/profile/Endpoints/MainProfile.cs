using System.Text.Json.Serialization;

namespace SovereigntyBot.Services.Endpoints.Profile
{
    public class MainProfile : IProfile
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("conditions")]
        public List<string> Conditions { get; set; }
    }
}