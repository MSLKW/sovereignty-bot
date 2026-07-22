using System;
using System.Text.Json.Serialization;

namespace SovereigntyBot.Services.Endpoints.Hypixel
{
    public class BuildBattle
    {
        [JsonPropertyName("wins")]
        public int Wins { get; set; }
        [JsonPropertyName("games_played")]
        public int Games { get; set; }
        [JsonPropertyName("score")]
        public int Score { get; set; }
    }
}