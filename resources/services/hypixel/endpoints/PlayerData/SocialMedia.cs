using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SovereigntyBot.Services.Endpoints.Hypixel
{
    public class SocialMedia
    {
        [JsonPropertyName("links")]
        public SocialMediaLinks Links { get; set; }
        [JsonPropertyName("prompt")]
        public bool prompt { get; set; }
        [JsonPropertyName("DISCORD")]
        public string DiscordServerLink { get; set; }
    }
}