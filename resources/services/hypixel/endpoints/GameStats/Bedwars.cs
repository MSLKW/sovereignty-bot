using System;
using System.Text.Json.Serialization;

namespace SovereigntyBot.Services.Endpoints.Hypixel
{
    public class Bedwars
    {
        [JsonPropertyName("wins_bedwars")]
        public int Wins { get; set; }
        [JsonPropertyName("losses_bedwars")]
        public int Losses { get; set; }
        [JsonPropertyName("kills_bedwars")]
        public int Kills { get; set; }
        [JsonPropertyName("deaths_bedwars")]
        public int Deaths { get; set; }
        [JsonPropertyName("final_kills_bedwars")]
        public int FinalKills { get; set; }
        [JsonPropertyName("final_deaths_bedwars")]
        public int FinalDeaths { get; set; }
        [JsonPropertyName("games_played_bedwars")]
        public int CoreGames { get; set; }
        [JsonPropertyName("games_played_bedwars_1")]
        public int OverallGames { get; set; }
        public double WLR { get{ return Wins/Math.Max(1, (double)Losses); }}
        public double KDR { get{ return Kills/Math.Max(1, (double)Deaths); }}
        public double FKDR { get{ return FinalKills/Math.Max(1, (double)FinalDeaths); }}
    }
}