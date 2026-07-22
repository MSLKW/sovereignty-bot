using System.Text.Json;
using System.Net;
using System.Net.Http.Json;
using SovereigntyBot.Services.Endpoints.Mojang;

namespace SovereigntyBot.Services
{
    public class MojangService
    {
        private static HttpClient _client = new HttpClient();
        private JsonSerializerOptions _options = new(){ WriteIndented = true };
        private CancellationTokenSource _source = new();
        private CancellationToken _token { get{ return _source.Token; } }
        // have a generic http request method that has null safety features and then returning the data

        // HTTP REQUESTS
 
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
                            await Program.Log(new Discord.LogMessage(Discord.LogSeverity.Warning, "MojangService.cs", "No content, invalid UUID?", exception));
                            break;
                    }
                }
                return null;
            }
        }
        private async Task<McPlayerData?> GetMcPlayerDataAsync(string uuid)
        {
            return await GetDataAsync<McPlayerData>($"https://sessionserver.mojang.com/session/minecraft/profile/{uuid}");
        }
        private async Task<McUuidData?> GetMcUuidDataAsync(string username)
        {
            return await GetDataAsync<McUuidData>($"https://api.mojang.com/users/profiles/minecraft/{username}");
        }

        // GET METHODS
        public async Task<string?> GetMcUsernameAsync(string uuid)
        {
            try
            {
                McPlayerData data = await GetMcPlayerDataAsync(uuid);
                return data.Name;
            }
            catch(NullReferenceException)
            {
                return null;
            }
        }

        public async Task<string?> GetUuidAsync(string username)
        {
            try
            {
                McUuidData data = await GetMcUuidDataAsync(username);
                return data.Id;
            }
            catch(NullReferenceException)
            {
                return null;
            }
        }
    }
}