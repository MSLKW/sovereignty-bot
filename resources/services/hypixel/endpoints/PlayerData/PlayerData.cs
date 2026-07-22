using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SovereigntyBot.Services.Endpoints.Hypixel
{
    public class PlayerData
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }
        [JsonPropertyName("player")]
        public Player Player { get; set; }
    }
}