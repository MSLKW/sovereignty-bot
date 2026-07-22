using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SovereigntyBot.Services.Endpoints.Hypixel
{
    public class Achievements
    {
        [JsonPropertyName("bedwars_level")]
        public int BedwarsStars { get; set; }
    }
}