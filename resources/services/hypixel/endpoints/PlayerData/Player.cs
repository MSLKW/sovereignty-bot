using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SovereigntyBot.Services.Endpoints.Hypixel
{
    public class Player
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; }
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }
        [JsonPropertyName("firstLogin")]
        public ulong FirstLogin { get; set; }
        [JsonPropertyName("playername")]
        public string PlayerName { get; set; }
        [JsonPropertyName("lastLogin")]
        public ulong LastLogin { get; set; }
        [JsonPropertyName("displayname")]
        public string DisplayName { get; set; }
        [JsonPropertyName("achievementPoints")]
        public int AchievementPoints { get; set; }
        [JsonPropertyName("stats")]
        public Stats Stats { get; set; }
        [JsonPropertyName("networkExp")]
        public double NetworkExp { get; set; }
        [JsonPropertyName("achievements")]
        public Achievements Achievements { get; set; }
        [JsonPropertyName("karma")]
        public ulong Karma { get; set; }
        [JsonPropertyName("lastLogout")]
        public ulong LastLogout { get; set; }
        [JsonPropertyName("socialMedia")]
        public SocialMedia SocialMedia { get; set; }
        public double NetworkLevel{
            get {
                return Math.Round((Math.Sqrt(((2 * this.NetworkExp) + 30625)) / 50) - 2.5, 2);
            }
        }
    }
}