using System.Text.Json.Serialization;

namespace SovereigntyBot.Services.Endpoints.Hypixel
{
    public class GuildMember
    {
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }
        [JsonPropertyName("rank")]
        public string Rank { get; set; }
        [JsonPropertyName("joined")]
        public ulong Joined { get; set; }
        [JsonPropertyName("expHistory")]
        public Dictionary<string, int> expHistory { get; set; }
        public int HighestGexpDay
        {
            get{
                int highest = 0;
                foreach(KeyValuePair<string, int> expPair in expHistory)
                {
                    if(expPair.Value > highest)
                    {
                        highest = expPair.Value;
                    }
                }
                return highest;
            }
        }
        public int CurrentGexpDay
        {
            get{
                return expHistory.ElementAt(0).Value;
            }
        }
        public int CurrentGexpWeek
        {
            get{
                int sum = 0;
                foreach(KeyValuePair<string, int> expPair in expHistory)
                {
                    sum += expPair.Value;
                }
                return sum;
            }
        }
        public DateTime JoinedDateTime {
            get{
                DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                dateTime = dateTime.AddMilliseconds((long)this.Joined);
                return dateTime;
            }
        } 
        public static ulong GetCurrentUnixTime()
			=> (ulong)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
        public ulong GetDurationSinceJoined()
        {
            return GetCurrentUnixTime() - (ulong)this.Joined;
        }
        public int GetDurationSinceJoinedInDays()
        {
            return (int)Math.Round((GetDurationSinceJoined() / (double)86400_000), 0);
        }

        // figure out a get method that doesn't include async becuz it returns null for some reason
    }
}