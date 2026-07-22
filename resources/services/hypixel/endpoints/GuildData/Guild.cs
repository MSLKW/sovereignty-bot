using System.Text.Json.Serialization;

namespace SovereigntyBot.Services.Endpoints.Hypixel
{
    public class Guild
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("created")]
        public ulong Created { get; set; }
        [JsonPropertyName("members")]
        public List<GuildMember> Members { get; set; }
        [JsonPropertyName("joinable")]
        public bool Joinable { get; set; }
        [JsonPropertyName("exp")]
        public ulong Exp { get; set; }
    }
}