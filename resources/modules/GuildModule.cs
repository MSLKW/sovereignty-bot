using Discord;
using Discord.WebSocket;
using Discord.Interactions;
using SovereigntyBot.Services.Endpoints.Hypixel;
using System.Text;
using SovereigntyBot.Services;
using SovereigntyBot.Services.Endpoints.Cache;
using SovereigntyBot.Services.Endpoints.Profile;
using SovereigntyBot.Services.Endpoints.Sovsmp;

namespace SovereigntyBot.Modules
{
    // Hypixel Guild Module
    [RequireUserPermission(GuildPermission.Administrator)]
    [Group("guild", "Hypixel Guild related commands")]
    public class GuildModule : InteractionModuleBase<SocketInteractionContext>
    {
        public SocketRole HypixelGuildMemberRole { get { return Context.Guild.GetRole(Program.ConfigData.GuildMemberRoleId); } }
        public IEnumerable<SocketGuildUser> discordGuildMemberUsers { get { return HypixelGuildMemberRole.Members; } }

        public bool IsGuildMemberInDiscord(string username) // user must have guild member role
        {
            foreach(SocketGuildUser discordGuildMemberUser in discordGuildMemberUsers)
            {
                string discordGuildMemberDisplayName = discordGuildMemberUser.DisplayName.ToLowerInvariant();
                // Program.Log(new Discord.LogMessage(Discord.LogSeverity.Verbose, "GuildModule.cs", $"discordGuildMember: {discordGuildMemberDisplayName}"));
                // Program.Log(new Discord.LogMessage(Discord.LogSeverity.Verbose, "GuildModule.cs", $"hypixelUsername: {username}"));
                if(discordGuildMemberDisplayName.Contains(username.ToLowerInvariant()))
                {
                    return true;
                }
            }
            return false;
        }
        [SlashCommand("prune", "Prune inactive Guild Members")]
        public async Task Prune()
        {
            var data = await Program.HypixelService.GetGuildDataByNameAsync(Program.GuildName);
            List<GuildMember> guildMembers = data.Guild.Members;

			List<SexpDailyEntry> sexpData = await Program.SovsmpService.GetSexpDataAsync();
            var table = new Table("Uuid", "Username", "WeeklyGexp", "JoinDate", "GuildRank", "InDiscord").WithPresetFields(new int[]{1, 2, 5});

			await Program.Log(new Discord.LogMessage(Discord.LogSeverity.Info, "GuildModule.cs", "Processing Guild Members"));
            foreach(GuildMember member in guildMembers)
            {
                var mpr = await ProfileService.InvokeMainProfileAsync("m003", member.Uuid, data);
                if(mpr.Success == false) continue;
                string name = await Program.MojangService.GetMcUsernameAsync(member.Uuid);
				// var sexpWeekly = SovsmpService.GetWeeklySexp(sexpData, member.Uuid);
				// await Program.Log(new Discord.LogMessage(Discord.LogSeverity.Info, "GuildModule.cs", $"Processing {name} | Sexp: {sexpWeekly}"));
				// if(sexpWeekly >= 3000)
				// {
				// 	await Program.Log(new Discord.LogMessage(Discord.LogSeverity.Info, "GuildModule.cs", $"{name} passed the SEXP requirement | SEXP: {sexpWeekly}\n"));
				// 	continue;
				// }
                var JoinDate = member.JoinedDateTime.ToLocalTime(); // ToLocalTime assuming GMT+0800
                var InDiscord = IsGuildMemberInDiscord(name);
                table.AddRow(member.Uuid, name, member.CurrentGexpWeek, JoinDate.ToString("yyyy-MM-dd, h:mm tt"), member.Rank, IsGuildMemberInDiscord(name));
                await Program.Log(new Discord.LogMessage(Discord.LogSeverity.Info, "GuildModule.cs", $"Adding {name}, {InDiscord}"));
            }
            
			StringBuilder sb = new StringBuilder();
            foreach(object[] result in table.Content)
            {
                // Name, Uuid, Rank, Gexp, JoinDate, InDiscord
                sb.Append($"{result[1]};{result[0]};{result[4]};{result[2]};{result[3] + " GMT+0800"};{((bool)result[5] == true ? "Yes" : "No")} \n");
            }
            Console.WriteLine(sb.ToString());

            await Program.Log(new Discord.LogMessage(Discord.LogSeverity.Info, "GuildModule.cs", "Creating Embed"));

            EmbedBuilder wEmbed = EmbedService.Success("Succesfully identified possible pruned guild members");
            var mcd = EmbedService.ModifyEmbedWithTable(wEmbed, table, new ComponentBuilder());
			var mentions = new AllowedMentions(AllowedMentionTypes.Roles);
			mentions.RoleIds = null;
			// Staff member RoleID: 1016843789786165308
			var message = await FollowupAsync(text: "<@&1016843789786165308>", embed: wEmbed.Build(), allowedMentions: mentions, components: mcd.wComponent.Build());
			Program.CacheService.Save(message.Id, new Cache(mcd));
        }

        [SlashCommand("promote", "Promote Guild Members")]
        public async Task Promote(string mainProfileId, string rankToPromote)
        {
            // NEED CHECKS FOR ARGUMENTS

            GuildData guildData = await Program.HypixelService.GetGuildDataByNameAsync(Program.GuildName);
            var table = new Table("Username", "Success");

            foreach(GuildMember member in guildData.Guild.Members)
            {
                if(member.Rank.ToLowerInvariant() != rankToPromote.ToLowerInvariant())
                {
                    continue;
                }
                MainProfileResult mpr = await ProfileService.InvokeMainProfileAsync(mainProfileId, member.Uuid, guildData);
                if(mpr.Success == true) // temporary filter
                {
                    string username = await Program.MojangService.GetMcUsernameAsync(member.Uuid);
                    table.AddRow(username, mpr.Success);
                }
            }

            table = table.WtihCodeblock(0);
            
            EmbedBuilder wEmbed = EmbedService.Success("Succesfully identified promotable Guild Members");
            var mcd = EmbedService.ModifyEmbedWithTable(wEmbed, table, new ComponentBuilder());
            var message = await FollowupAsync(embed: wEmbed.Build(), components: mcd.wComponent.Build());
            
            Program.CacheService.Save(message.Id, new Cache(mcd));
        }

        [SlashCommand("check", "Check join requirements")]
        public async Task CheckJoinRequirementsSlash(string username)
        {
            string uuid = await Program.MojangService.GetUuidAsync(username);
            if(uuid == null)
            {
                await FollowupAsync($"{username} does not exist on Hypixel", ephemeral: true);
                return;
            }
            MainProfileResult mpr = await ProfileService.InvokeMainProfileAsync("m002", uuid);

            if(mpr.Success == true)
            {
                await FollowupAsync($"{username} is eligible to join the guild", ephemeral: true);
            }
            else if(mpr.Success == false)
            {
                await FollowupAsync($"{username} is NOT eligible to join the guild", ephemeral: true);
            }
        }

        [UserCommand("Check Join Requirements")]
        public async Task CheckJoinRequirements(IUser user)
        {
            SocketGuildUser discordUser = user as SocketGuildUser;
            
            // figure out how to verify discordUser.DisplayName

            string uuid = await Program.MojangService.GetUuidAsync(discordUser.DisplayName);
            if(uuid == null)
            {
                await FollowupAsync($"{discordUser.DisplayName} does not exist on Hypixel", ephemeral: true);
                return;
            }
            MainProfileResult mpr = await ProfileService.InvokeMainProfileAsync("m002", uuid);

            if(mpr.Success == true)
            {
                await FollowupAsync($"{discordUser.DisplayName} is eligible to join the guild", ephemeral: true);
            }
            else if(mpr.Success == false)
            {
                await FollowupAsync($"{discordUser.DisplayName} is not eligible to join the guild", ephemeral: true);
            }
        }
    }
}