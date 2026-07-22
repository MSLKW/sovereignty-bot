using System.Text.Json.Serialization;

namespace SovereigntyBot.Services.Endpoints.Profile
{
    public class ConditionProfile : IProfile
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("statistic")]
        public string Statistic { get; set; }
        [JsonPropertyName("operator")]
        public string Operator { get; set; }
        [JsonPropertyName("value")]
        public int Value { get; set; }
    }
}