using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SovereigntyBot.Services.Endpoints.Hypixel
{
    public class Stats
    {
        [JsonPropertyName("achievements")]
        public Achievements Achievements { get; set; }
        [JsonPropertyName("Duels")]
        public Duels Duels { get; set; }
        [JsonPropertyName("MurderMystery")]
        public MurderMystery MurderMystery { get; set; }
        [JsonPropertyName("SkyWars")]
        public SkyWars Skywars { get; set; }
        [JsonPropertyName("Bedwars")]
        public Bedwars Bedwars { get; set; }
        [JsonPropertyName("BuildBattle")]
        public BuildBattle BuildBattle { get; set; }
        [JsonPropertyName("Arcade")]
        public Arcade Arcade { get; set; }
        // [JsonPropertyName("UHC")]
        // public UHC UHC { get; set; }
    }
}