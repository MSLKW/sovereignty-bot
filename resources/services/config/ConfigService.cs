using Discord;
using System.Text.Json;
using SovereigntyBot.Services.Endpoints.Config;

namespace SovereigntyBot.Services
{
    public class ConfigService
    {
        private string _rootPath = @"./resources/services/config/";
        private string _fileName = "liveconfig";
        private string _filePath { 
            get { return _rootPath + _fileName + ".json"; } 
        }

        private RootConfig defaultData
        {
            get {
                return new RootConfig(){
                    HypixelApiKey = "NULL",
					SovSmpApiKey = "NULL",
                    DiscordToken = "NULL",
                    GuildMemberRoleId = 000000000000000000,
                    DiscordMemberRoleId = 000000000000000000,
                    RankRoleIds = new ulong[]{000000000000000000, 000000000000000000, 000000000000000000},
                    DiscordServerId = 000000000000000000
                };
            }
        }

        public async Task<RootConfig> ReadAsync()
        {
            try
            {
                using(FileStream fs = File.OpenRead(_filePath))
                {
                    RootConfig root = await JsonSerializer.DeserializeAsync<RootConfig>(fs);
                    return root;
                }
            }
            catch(JsonException except)
            {
                await Program.Log(new LogMessage(LogSeverity.Error, "ConfigService.cs", except.Message, except));
                await WriteAsync(defaultData);
                return null;
            }
            catch(FileNotFoundException except)
            {
                await Program.Log(new LogMessage(LogSeverity.Error, "ConfigService.cs", except.Message, except));
                await WriteAsync(defaultData);
                return null;
            }
        }

        public async Task WriteAsync(RootConfig data)
        {
            try
            {
                var options = new JsonSerializerOptions{ WriteIndented = true };
                using(FileStream fs = File.OpenWrite(_filePath))
                {
                    await JsonSerializer.SerializeAsync(fs, data, options);
                }
            }
            catch(ArgumentNullException except)
            {
                await Program.Log(new LogMessage(LogSeverity.Error, "ConfigService.cs", except.Message, except));
            }
        }
    }
}