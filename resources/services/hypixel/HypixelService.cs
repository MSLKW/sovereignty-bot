using Discord;
using System.Text.Json;
using System.Net;
using System.Net.Http.Json;
using SovereigntyBot.Services.Endpoints.Hypixel;

namespace SovereigntyBot.Services
{
    public class HypixelService
    {
        private HttpClient _client;
        private string _apiKey;
        private JsonSerializerOptions _options = new(){ WriteIndented = true };
        private CancellationTokenSource _source = new();
        private CancellationToken _token { get{ return _source.Token; } }

        public HypixelService(string apiKey)
        {
            _apiKey = apiKey; // get a check for this
            _client = new HttpClient();
        }
        // API FUNCTIONS
        private async Task<DataType?> GetDataAsync<DataType>(string httpRequest) where DataType : class
        {
            try
            {
                DataType data = await _client.GetFromJsonAsync<DataType>(httpRequest, _options, _token);
                return data;
            }
            catch(HttpRequestException exception)
            {
                if(exception.StatusCode.HasValue == true)
                {
                    HttpStatusCode statusCode = exception.StatusCode.Value;
                    switch(statusCode)
                    {
                        case HttpStatusCode.NoContent:
                            await Program.Log(new LogMessage(LogSeverity.Warning, "HypixelService.cs", "No content, invalid argument?", exception));
                            break;
                        case HttpStatusCode.Forbidden:
                            await Program.Log(new LogMessage(LogSeverity.Warning, "HypixelService.cs", "Access Forbidden, Invalid Api Key", exception));
                            break;
                        case HttpStatusCode.TooManyRequests:
                            await Program.Log(new LogMessage(LogSeverity.Warning, "HypixelService.cs", "Too many requests", exception));
                            break;
                    }
                }
                return null;
            }
        }
        public async Task<KeyData> GetKeyDataAsync()
        {
            return await GetDataAsync<KeyData>($"https://api.hypixel.net/key?key={_apiKey}");
        }
        public async Task<GuildData> GetGuildDataByNameAsync(string guildName)
        {
            return await GetDataAsync<GuildData>($"https://api.hypixel.net/guild?key={_apiKey}&name={guildName}");
        }
        public async Task<PlayerData> GetPlayerDataByUuidAsync(string uuid)
        {
            return await GetDataAsync<PlayerData>($"https://api.hypixel.net/player?key={_apiKey}&uuid={uuid}");
        }
        public async Task<PlayerData> GetPlayerDataByNameAsync(string username)
        {
            string uuid = await Program.MojangService.GetUuidAsync(username);
            return await GetPlayerDataByUuidAsync(uuid);
        }

        // Helper Functions 

        public async static Task<Dictionary<string, string>> GetGuildMemberNamesAsync(List<GuildMember> guildMemberList)
        {
            Dictionary<string, string> UsernamesToUuid = new();

            // iterate through member list
            for(int i=0; i < guildMemberList.Count; i++)
            {
                GuildMember member = guildMemberList[i];
                // get ign from each member's uuid
                string memberIgn = await Program.MojangService.GetMcUsernameAsync(member.Uuid);
                // put ign into guildMemberNames
                UsernamesToUuid[memberIgn] = member.Uuid;
            }

            return UsernamesToUuid;
        }

        public static GuildMember? GetGuildMember(string uuid, GuildData data)
        {
            try
            {
                foreach(GuildMember member in data.Guild.Members)
                {
                    if(member.Uuid == uuid)
                    {
                        return member;
                    }
                }   
            }
            catch(NullReferenceException) {}
            return null;
        }
    }
}