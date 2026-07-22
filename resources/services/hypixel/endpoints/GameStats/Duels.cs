using System;
using System.Text.Json.Serialization;

namespace SovereigntyBot.Services.Endpoints.Hypixel
{
    public class Duels
    {
        [JsonPropertyName("wins")]
        public int Wins { get; set; }
        [JsonPropertyName("losses")]
        public int Losses { get; set; }
        [JsonPropertyName("kills")]
        public int Kills { get; set; }
        [JsonPropertyName("deaths")]
        public int Deaths { get; set; }
        [JsonPropertyName("best_overall_winstreak")]
        public int BestOverallWinstreak { get; set; }
        [JsonPropertyName("games_played_duels")]
        public int Games { get; set; }
        public int KDR { get{ return Kills/Math.Max(1, Deaths); }}
        public int WLR { get{ return Wins/Math.Max(1, Losses); }}
    }
}
