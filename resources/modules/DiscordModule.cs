using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using SovereigntyBot.Services;
using SovereigntyBot.Services.Endpoints.Cache;
using SovereigntyBot.Services.Endpoints.Hypixel;
using SovereigntyBot.Modules.Results;

namespace SovereigntyBot.Modules
{
    [RequireUserPermission(GuildPermission.Administrator)]
    [Group("discord", "Discord Server related commands")]
    public class DiscordModule : InteractionModuleBase<SocketInteractionContext>
    {
        public SocketRole HypixelGuildMemberRole { get { return Context.Guild.GetRole(Program.ConfigData.GuildMemberRoleId); } }
        public SocketRole DiscordMemberRole { get { return Context.Guild.GetRole(Program.ConfigData.DiscordMemberRoleId); }}
        const ulong CACHE_ID = 0;
        public List<SocketRole> RankRoles { 
            get {
                var rankRoleIds = Program.ConfigData.RankRoleIds;
                List<SocketRole> roles = new List<SocketRole>();
                for(int i=0; i < rankRoleIds.Length; i++)
                {
                    SocketRole role = Context.Guild.GetRole(rankRoleIds[i]);
                    roles.Add(role);
                }
                return roles;
            }
        }

        public SocketRole GetRankRole(SocketGuildUser user)
        {
            foreach(SocketRole role in RankRoles)
            {
                if(user.Roles.Contains(role))
                {
                    return role;
                }
            }
            return null;
        }

        public SocketRole GetRankRole(GuildMember user)
        {
            foreach(SocketRole role in RankRoles)
            {
                if(user.Rank.ToLowerInvariant() == role.Name.ToLowerInvariant())
                {
                    return role;
                }
            }
            return null;
        }

        // Is User with Guild Member Role in Discord is in Hypixel Guild
        private static DiscordUserInGuildResult IsDiscordUserInGuildAsync(SocketGuildUser discordUser, Dictionary<string, string> usernamesToUuid, GuildData data)
		{
			DiscordUserInGuildResult results = new();
			results.DiscordUser = discordUser;

			string discordUserDisplayNameLowered = discordUser.DisplayName.ToLowerInvariant();

            string[] guildMemberNames = usernamesToUuid.Keys.ToArray();
            List<string> guildMemberNamesLowered = guildMemberNames.Select(x => x.ToLowerInvariant()).ToList();
            
			// Checking if DiscordUser is in Guild List by Server Name
			int index = guildMemberNamesLowered.IndexOf(discordUserDisplayNameLowered);
			if(index != -1) // True
			{
                // UsernamesToUuid[guildMemberNames[index]] only provides Hypixel Usernames that are in Guild Members
				results.GuildMember = HypixelService.GetGuildMember(usernamesToUuid[guildMemberNames[index]], data);
                results.HypixelUsername = guildMemberNames[index];
				results.Success = true;
                results.isLong = false;
			}
			else if(index == -1) // False
			{
				bool isLongName = false;
				// Checking for Long Names
				foreach(string guildMemberNameLowered in guildMemberNamesLowered)
				{
					if(discordUserDisplayNameLowered.Contains(guildMemberNameLowered))
					{
                        index = guildMemberNamesLowered.IndexOf(guildMemberNameLowered);
						results.GuildMember = HypixelService.GetGuildMember(usernamesToUuid[guildMemberNames[index]], data);
                        results.HypixelUsername = guildMemberNames[index];
                        isLongName = true;
						break;
					}
				}

				if(isLongName == true)
				{
					results.Success = true;
					results.isLong = true;
				}
				else if(isLongName == false)
				{
					results.GuildMember = null;
                    results.HypixelUsername = null;
					results.Success = false;
                    results.isLong = false;
				}
			}
			return results;
		}

        [SlashCommand("identify", "Identify Ex-Guild Members")]
        public async Task Identify()
        {
            List<DiscordUserInGuildResult> results = new List<DiscordUserInGuildResult>();
            // check discord users with guild member role that is not in the guild 

            // get discord users with guild member role 

            // get guild member list from hypixel guild data

            var guildData = await Program.HypixelService.GetGuildDataByNameAsync(Program.GuildName);
            var guildMemberList = guildData.Guild.Members;
            // create a guild member list from guild data

            Dictionary<string, string> usernamesToUuid = await HypixelService.GetGuildMemberNamesAsync(guildData.Guild.Members);
            // iterate and check if discord user is in guild member list
            foreach(SocketGuildUser user in HypixelGuildMemberRole.Members)
            {
                DiscordUserInGuildResult result = IsDiscordUserInGuildAsync(user, usernamesToUuid, guildData);

                results.Add(result);
            }

            // figure out how to cache IdentifyResult results for "purge" use later.
            Program.CacheService.Save(CACHE_ID, new Cache(results));
            await Program.Log(new LogMessage(LogSeverity.Info, "DiscordModule.cs", $"Cached results to {CACHE_ID}"));

            var table = new Table("Discord Username", "Success", "Hypixel Username");
            foreach(DiscordUserInGuildResult result in results)
            {
				if (result.Success == false)
                	table.AddRow($"`{result.DiscordUser.DisplayName}`", result.Success, $"`{result.HypixelUsername}`");
            }

            // table = table.WithFilter(1, success => { return (bool)success == false; } );

            var wEmbed = EmbedService.Success("Use /discord purge to remove roles");
            var mcd = EmbedService.ModifyEmbedWithTable(wEmbed, table, new ComponentBuilder());
            var message = await FollowupAsync(embed: mcd.wEmbed.Build(), components: mcd.wComponent.Build());
            Program.CacheService.Save(message.Id, new Cache(mcd));
        }

        [SlashCommand("purge", "Remove Guild Member Role from Ex-Guild Members")]
        public async Task Purge(ulong id)
        {
            var table = new Table("Discord Username", "Rank Role");

            List<DiscordUserInGuildResult> resultsCache = (List<DiscordUserInGuildResult>)Program.CacheService.Load(id).Data;
            if(resultsCache == null)
            {
                await Program.Log(new LogMessage(LogSeverity.Warning, "DiscordModule.cs", "IdentifyCache not found"));
                await FollowupAsync(embed: EmbedService.Error("IdentifyCache not found, have you used /discord identify yet?").Build());
                return;
            }

            foreach(DiscordUserInGuildResult result in resultsCache)
            {
				await Program.Log(new LogMessage(LogSeverity.Debug, "DiscordModule.cs", $"iterating resultsCache"));
				if(result.Success == false) // replace with filter
                {
                    SocketRole rankRole = GetRankRole(result.DiscordUser);

                    if(rankRole == RankRoles[4]) // Check for Wowiee best boy
                    {
                        await Program.Log(new LogMessage(LogSeverity.Warning, "DiscordModule.cs", $"Detected Wowiee Best Boy, Continuing... | {result.DiscordUser.DisplayName}"));
                        continue;
                    }
                    
                    await result.DiscordUser.RemoveRoleAsync(rankRole);
                    await result.DiscordUser.RemoveRoleAsync(HypixelGuildMemberRole);
                    await result.DiscordUser.AddRoleAsync(DiscordMemberRole);

                    table.AddRow($"`{result.DiscordUser.DisplayName}`", rankRole.Name);
                }
            }

            Program.CacheService.Delete(CACHE_ID);
            
            var wEmbed = EmbedService.Success("Purged Ex Guild Members and Reseted Cache");
            var mcd = EmbedService.ModifyEmbedWithTable(wEmbed, table, new ComponentBuilder());

            var message = await FollowupAsync(embed: mcd.wEmbed.Build(), components: mcd.wComponent.Build());

            Program.CacheService.Save(message.Id, new Cache(mcd));
        }

        [SlashCommand("update-rank", "Update Rank Roles")]
        public async Task UpdateRankRole()
        {
            Table table = new Table("Discord Username", "Success", "Previous Rank", "Updated Rank");
            var guildData = await Program.HypixelService.GetGuildDataByNameAsync(Program.GuildName);

            // get guild members in discord through guild member role ( guild member role member list )
            List<DiscordUserInGuildResult> results = new List<DiscordUserInGuildResult>();

            Dictionary<string, string> usernamesToUuid = await HypixelService.GetGuildMemberNamesAsync(guildData.Guild.Members);
            foreach(SocketGuildUser user in HypixelGuildMemberRole.Members)
            {
                DiscordUserInGuildResult result = IsDiscordUserInGuildAsync(user, usernamesToUuid, guildData);

                results.Add(result);
            }

            // get rank from guild api

            foreach(DiscordUserInGuildResult result in results)
            {
                await Program.Log(new LogMessage(LogSeverity.Debug, "DiscordModule.cs", $"{result.DiscordUser.DisplayName}"));
                if(result.Success == false)
                {
                    continue;
                }

                // nullable
                SocketRole hypixelRankRole = GetRankRole(result.GuildMember);
                SocketRole discordRankRole = GetRankRole(result.DiscordUser);

                if(hypixelRankRole == null || discordRankRole == null)
                {
                    // table.AddRow(result.DiscordUser.DisplayName, false, null, null);
                    continue;
                }

                if(hypixelRankRole == discordRankRole)
                {
                    // table.AddRow(result.DiscordUser.DisplayName, false, discordRankRole.Name, hypixelRankRole.Name);
                }
                else if(hypixelRankRole != discordRankRole)
                {
                    await result.DiscordUser.RemoveRoleAsync(discordRankRole);
                    await result.DiscordUser.AddRoleAsync(hypixelRankRole);

                    table.AddRow(result.DiscordUser.DisplayName, true, discordRankRole.Name, hypixelRankRole.Name);
                }
            }

            // table = table.WithFilter(1, success => { return (bool)success == true;} );
            
            var wEmbed = EmbedService.Success("Updated Discord Users Rank");
            var mcd = EmbedService.ModifyEmbedWithTable(wEmbed, table, new ComponentBuilder());

            var message = await FollowupAsync(embed: mcd.wEmbed.Build(), components: mcd.wComponent.Build());

            Program.CacheService.Save(message.Id, new Cache(mcd));
        }

		// [SlashCommand()]
		// public async Task IsGuildMemberInDiscord()
		// {

		// }
    }
}