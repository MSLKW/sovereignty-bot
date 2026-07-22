using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SovereigntyBot.Services.Endpoints.Hypixel
{
    public class SocialMediaLinks
    {
        [JsonPropertyName("DISCORD")]
        public string Discord { get; set; }
        [JsonPropertyName("HYPIXEL")]
        public string Hypixel { get; set; }
    }
}