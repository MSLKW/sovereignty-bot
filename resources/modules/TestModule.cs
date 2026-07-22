using Discord;
using Discord.WebSocket;
using Discord.Interactions;
using SovereigntyBot.Services;
using SovereigntyBot.Services.Endpoints.Cache;
using SovereigntyBot.Modules.Results;

namespace SovereigntyBot.Modules
{
    [RequireUserPermission(GuildPermission.Administrator)]
    public class TestModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("read", "echo an input")]
        public async Task ReadJson()
        {
            var uuid = await Program.MojangService.GetUuidAsync("testotijsetoijseoji"); // need to somehow just continue on
            await Program.Log(new LogMessage(LogSeverity.Info, "TestModule.cs", $"uuid: {uuid}"));
        }

        [SlashCommand("test", "testing")]
        public async Task TestFunction()
        {
            var table = new Table("Field1", "Field2", "Field3", "Field4");
            for(int i=0; i<100; i++)
            {
                table.AddRow("yum", i, i ,i);
            }

            table = table.WtihCodeblock(0);
            
            var wEmbed = EmbedService.Success("test");
            var mcd = EmbedService.ModifyEmbedWithTable(wEmbed, table, new ComponentBuilder());
            var message = await FollowupAsync(embed: mcd.wEmbed.Build(), components: mcd.wComponent.Build());

            Program.CacheService.Save(message.Id, new Cache(mcd));

            // ModalBuilder wModal = new ModalBuilder()
            //     .WithCustomId("modal-test")
            //     .WithTitle("Testing modal")
            //     .AddTextInput("Cool label 1", "modal-test-label-1", TextInputStyle.Short, "Epic placeholder", 0, 50, true)
            //     .AddTextInput("Cool label 2", "modal-test-label-2", TextInputStyle.Paragraph, "You're hawt", 0, 1000, true);


            // await RespondWithModalAsync(wModal.Build()); // cannot be defered
        }
    }
}