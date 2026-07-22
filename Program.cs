using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord.Interactions;
using System.Text.Json.Serialization;
using System.Reflection;
using SovereigntyBot.Services;
using SovereigntyBot.Services.Endpoints.Config;
using SovereigntyBot.Handlers;
using SovereigntyBot.Modules.Results;

namespace SovereigntyBot
{
    class Program
    {
        private DiscordSocketClient _client;
        private InteractionService _interactionService;
        public static ConfigService ConfigService;
        public static HypixelService HypixelService;
        public static MojangService MojangService;
        public static CacheService CacheService;
		public static SovsmpService SovsmpService;
        public static RootConfig ConfigData;
        public static string GuildName = "sovereignty";
        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        private async Task MainAsync()
        {
            // Discord Client and Event Subscription
            var discordConfig = new DiscordSocketConfig(){
                AlwaysDownloadUsers = true,
                GatewayIntents = GatewayIntents.All
            };
            _client = new DiscordSocketClient(discordConfig);

            _client.Log += Log;
            _client.Ready += ReadyHandler;
            _client.ButtonExecuted += ButtonHandler.Handle;

            // Setting Up Custom Services
            ConfigService = new ConfigService();
            ConfigData = await ConfigService.ReadAsync();

            HypixelService = new HypixelService(ConfigData.HypixelApiKey);
			SovsmpService = new SovsmpService(ConfigData.SovSmpApiKey);
            MojangService = new MojangService();
            CacheService = new CacheService();


            // Setting Up Discord Bot

            await _client.LoginAsync(TokenType.Bot, ConfigData.DiscordToken);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task ReadyHandler()
        {
            _interactionService = new InteractionService(_client.Rest);

            _interactionService.SlashCommandExecuted += SlashCommandHandler;
            _interactionService.ContextCommandExecuted += ContextCommandHandler;

			await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), null);
			await _interactionService.RegisterCommandsToGuildAsync(ConfigData.DiscordServerId);

            _client.InteractionCreated += CreateInteraction;
        }

        private async Task CreateInteraction(SocketInteraction interaction)
        {
            var ctx = new SocketInteractionContext(_client, interaction);

            // how to see if it's a slash command or not
            if(interaction is ISlashCommandInteraction slashCommand)
            {
                // slashCommand.Data.Name only provides name of module? 
                // e.x. : "guild prune" would only give "guild"
                await ctx.Interaction.DeferAsync();
                await Program.Log(new LogMessage(LogSeverity.Info, "Program.cs", $"Starting a Command | Name: {slashCommand.Data.Name}"));
            }
            else if(interaction is IUserCommandInteraction userCommand)
            {
                await ctx.Interaction.DeferAsync();
                await Program.Log(new LogMessage(LogSeverity.Info, "Program.cs", $"Starting a Context Command | Name: {userCommand.Data.Name}"));
            }

            await _interactionService.ExecuteCommandAsync(ctx, null);
        }

        public static Task Log(LogMessage msg)
        {
            // if(msg.Severity == LogSeverity.Verbose) return Task.CompletedTask;
            
            Console.WriteLine(msg.ToString());
            
            return Task.CompletedTask;
        }

        private async static Task SlashCommandHandler(SlashCommandInfo info, Discord.IInteractionContext ctx, IResult result)
        {
            if(result.IsSuccess == false)
            {
                switch(result.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        await ctx.Interaction.RespondAsync(embed: EmbedService.Error($"Unmet Precondition: {result.ErrorReason}").Build());
                        break;
                    case InteractionCommandError.UnknownCommand:
                        await ctx.Interaction.RespondAsync(embed: EmbedService.Error("Unknown command").Build());
                        break;
                    case InteractionCommandError.BadArgs:
                        await ctx.Interaction.RespondAsync(embed: EmbedService.Error("Invalid number or arguments").Build());
                        break;
                    case InteractionCommandError.Exception:
                        await ctx.Interaction.RespondAsync(embed: EmbedService.Error($"Command exception: {result.ErrorReason}").Build());
                        break;
                    case InteractionCommandError.Unsuccessful:
                        await ctx.Interaction.RespondAsync(embed: EmbedService.Error($"Command could not be executed: {result.ErrorReason}").Build());
                        break;
                    default:
                        await Program.Log(new LogMessage(LogSeverity.Info, $"Program.cs", $"Defaulted when success is error"));
                        break;
                }
            }
            else if(result.IsSuccess == true)
            {
                await Program.Log(new LogMessage(LogSeverity.Info, $"Program.cs", $"{info.Module.Name} | {info.MethodName} successfully executed"));
            }
        }

        private async static Task ContextCommandHandler(ContextCommandInfo info, IInteractionContext ctx, IResult result)
        {
            if(result.IsSuccess == true)
            {
                await Program.Log(new LogMessage(LogSeverity.Info, $"Program.cs", $"{info.Module.Name} | {info.MethodName} successfully executed"));
            }
        }
    }
}