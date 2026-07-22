using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace SovereigntyBot.Services.Endpoints.Mojang
{
    public class McNameHistory
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("changedToAt")]
        public ulong ChangedToAt { get; set; }
    }
}