using System.Text.Json.Serialization;

namespace SovereigntyBot.Services.Endpoints.Config
{
    public class RootConfig
    {
        [JsonPropertyName("hypixel_api_key")]
        public string HypixelApiKey { get; set; }
		[JsonPropertyName("smp_api_key")]
		public string SovSmpApiKey { get; set; }
        [JsonPropertyName("discord_token")]
        public string DiscordToken { get; set; }
        [JsonPropertyName("guild_member_role_id")]
        public ulong GuildMemberRoleId { get; set; }
        [JsonPropertyName("discord_member_role_id")]
        public ulong DiscordMemberRoleId { get; set; }
        // RankRoleIds is from ascending order from bottom to top [ Boye, Good Boiks, Real Good Boys, Extra Gud Boiz, Wowiee Best Boy]
        [JsonPropertyName("rank_role_ids")]
        public ulong[] RankRoleIds { get; set; }
        [JsonPropertyName("discord_server_id")]
        public ulong DiscordServerId { get; set; }
    }
}