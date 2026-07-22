using System.Text.Json.Serialization;

namespace SovereigntyBot.Services.Endpoints.Profile
{
    public class RootProfile
    {
        [JsonPropertyName("m")]
        public List<MainProfile> MainProfiles { get; set; }
        [JsonPropertyName("g")]
        public List<GroupProfile> GroupProfiles { get; set; }
        [JsonPropertyName("c")]
        public List<ConditionProfile> ConditionProfiles { get; set; }
        [JsonPropertyName("ids")]
        public List<string> Ids { get; set; }
    }
}