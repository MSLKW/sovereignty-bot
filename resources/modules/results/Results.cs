using Discord.WebSocket;
using SovereigntyBot.Services;
using SovereigntyBot.Services.Endpoints.Hypixel;

namespace SovereigntyBot.Modules.Results
{
    public struct PurgeResult
    {
        public bool Success;
        public SocketGuildUser SocketDiscordUser;
        public SocketRole RankRole;
    }

    public struct DiscordUserInGuildResult
    {
        public SocketGuildUser DiscordUser;
        public GuildMember? GuildMember;
        public string? HypixelUsername;
        public bool Success;
        public bool isLong;
    }
}