using System.Text.Json.Serialization;

namespace SovereigntyBot.Services.Endpoints.Profile
{
    public interface IProfile
    {
        [JsonPropertyName("name")]
        string Name { get; set; }
        [JsonPropertyName("id")]
        string Id { get; set; }
    }
}