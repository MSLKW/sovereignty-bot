using System;
using System.Text.Json.Serialization;

namespace SovereigntyBot.Services.Endpoints.Hypixel
{
    public class SkyWars
    {
        [JsonPropertyName("wins")]
        public int Wins { get; set; }
        [JsonPropertyName("losses")]
        public int Losses { get; set; }
        [JsonPropertyName("kills")]
        public int Kills { get; set; }
        [JsonPropertyName("deaths")]
        public int Deaths { get; set; }
        [JsonPropertyName("games_played_skywars")]
        public int Games { get; set; }
        public double WLR { get {return Wins / Math.Max(1, Losses);}}
        public double KDR { get {return Kills / Math.Max(1, Deaths);}}
        [JsonPropertyName("skywars_experience")]
        public double Exp { get; set; }
        public double Stars { 
            get{
                double xp = this.Exp;
                int[] xps = {0, 20, 70, 150, 250, 500, 1000, 2000, 3500, 6000, 10000, 15000};
                if(xp >= 15000) 
                {
                    return (xp - 15000) / 10000 + 12;
                } 
                else 
                {
                    for(int i = 0; i < xps.Length; i++) 
                    {
                        if(xp < xps[i]) 
                        {
                            return i + (xp - xps[i-1]) / (xps[i] - xps[i-1]);
                        }
                    }
                }
                return 0;
            }
        }
    }
}