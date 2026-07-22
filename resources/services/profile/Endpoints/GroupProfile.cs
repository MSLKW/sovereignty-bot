using System.Text.Json.Serialization;

namespace SovereigntyBot.Services.Endpoints.Profile
{
    public class GroupProfile : IProfile
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("conditions")]
        public List<string> Conditions { get; set; }
        [JsonPropertyName("totalConditionsPass")]
        public int TotalConditionsPass { get; set; } // total amount of groups to pass to pass rqp
    }
}