using System.Text.Json;
using SovereigntyBot.Services.Endpoints.Profile;
using SovereigntyBot.Services.Endpoints.Hypixel;
using Discord;

namespace SovereigntyBot.Services
{
    public class ProfileService
    {
        private static string _filePath = @"./resources/services/profile/profile.json";
        
        private async static Task WriteAsync(RootProfile data)
        {
            var options = new JsonSerializerOptions{ WriteIndented = true };
            using(FileStream fs = File.OpenWrite(_filePath))
            {
                await JsonSerializer.SerializeAsync(fs, data, options);
            }
        }   

        private async static Task<RootProfile> ReadAsync()
        {
            using(FileStream fs = File.OpenRead(_filePath))
            {
                RootProfile data = await JsonSerializer.DeserializeAsync<RootProfile>(fs);
                return data;
            }
        }

        private async static Task<MainProfile> CreateMainProfileAsync(string name, List<string> conditions)
        {
            MainProfile mp = new MainProfile();
            mp.Name = name;
            mp.Id = await ProfileService.GenerateIdAsync('m');
            mp.Conditions = conditions;
            return mp;
        }

        private async static Task<GroupProfile> CreateGroupProfileAsync(string name, List<string> conditions, int totalConditionsPass)
        {
            GroupProfile gp = new GroupProfile();
            gp.Name = name;
            gp.Id = await ProfileService.GenerateIdAsync('g');
            gp.Conditions = conditions;
            gp.TotalConditionsPass = totalConditionsPass;
            return gp;
        }

        private async static Task<ConditionProfile> CreateConditionProfileAsync(string name, string statistic, string _operator, int value)
        {
            ConditionProfile cp = new ConditionProfile();
            cp.Name = name;
            cp.Id = await ProfileService.GenerateIdAsync('c');
            cp.Statistic = statistic;
            cp.Operator = _operator;
            cp.Value = value;
            return cp;
        }
        // generate id

        private async static Task<string> GenerateIdAsync(char profileType)
        {
            RootProfile rp = await ProfileService.ReadAsync();
            for(int i=1; i < 1000; i++)
            {
                string idName = $"{profileType}{i.ToString("D3")}";
                if(rp.Ids.Count < 1 || rp.Ids.Contains(idName) == false)
                {
                    rp.Ids.Add(idName);
                    await ProfileService.WriteAsync(rp);
                    return idName;
                }
                else if(idName == $"{profileType}999")
                {
                    await Program.Log(new LogMessage(LogSeverity.Error, "ProfileService.cs", "Maximum amount of profiles reached."));
                }
            }
            throw new IndexOutOfRangeException();
        }
        // load profiles

        private async static Task<MainProfile> LoadMainProfileAsync(string id)
        {
            RootProfile rp = await ProfileService.ReadAsync();
            foreach(MainProfile mp in rp.MainProfiles)
            {
                if(mp.Id == id)
                {
                    return mp;
                }
            }
            return null;
        }

        private async static Task<GroupProfile> LoadGroupProfileAsync(string id)
        {
            RootProfile rp = await ProfileService.ReadAsync();
            foreach(GroupProfile gp in rp.GroupProfiles)
            {
                if(gp.Id == id)
                {
                    return gp;
                }
            }
            return null;
        }
        
        private async static Task<ConditionProfile> LoadConditionProfileAsync(string id)
        {
            RootProfile rp = await ProfileService.ReadAsync();
            foreach(ConditionProfile cp in rp.ConditionProfiles)
            {
                if(cp.Id == id)
                {
                    return cp;
                }
            }
            return null;
        }

        private static bool? CheckCondition(string _operator, double? inputValue, double conditionValue)
        {
            if(inputValue == null)
            {
                Program.Log(new LogMessage(LogSeverity.Error, "ProfileService.cs", "Input Value is null"));
                return null;
            }
            switch(_operator)
            {
                case "<":
                    return inputValue < conditionValue;
                case "<=":
                    return inputValue <= conditionValue;
                case "==":
                    return inputValue == conditionValue;
                case ">=":
                    return inputValue >= conditionValue;
                case ">":
                    return inputValue > conditionValue;
                default:
                    Program.Log(new LogMessage(LogSeverity.Error, "ProfileService.cs", "Operator not found"));
                    return null;
            }
        }
        // statistic interpreter (turn condition strings into actual api calls)
        private static double? GetStatistic(string statistic, string uuid, PlayerData playerData, GuildMember? guildMember)
        {
            try
            {
                switch(statistic)
                {
                    // Hypixel General
                    case "network_level":
                        return playerData.Player.NetworkLevel;
                    // Bedwars
                    case "bedwars_stars":
                        return playerData.Player.Achievements.BedwarsStars;
                    case "bedwars_fkdr":
                        return playerData.Player.Stats.Bedwars.FKDR;
                    case "bedwars_wins":
                        return playerData.Player.Stats.Bedwars.Wins;
                    // Skywars
                    case "skywars_level":
                        return playerData.Player.Stats.Skywars.Stars;
                    case "skywars_wins":
                        return playerData.Player.Stats.Skywars.Wins;
                    // Duels
                    case "duels_wins":
                        return playerData.Player.Stats.Duels.Wins;
                    case "duels_wlr":
                        return playerData.Player.Stats.Duels.WLR;
                    // Gexp
                    case "current_gexp_week":
                        return guildMember.CurrentGexpWeek;
                    case "highest_gexp_day_db":
                        return null;
                    case "highest_gexp_week_db":
                        return null;  
                    // Guild
                    case "since_joined_in_days":
                        return guildMember.GetDurationSinceJoinedInDays();   

                    default:
                        Program.Log(new LogMessage(LogSeverity.Error, "ProfileService.cs", $"Statistic not found | Stat: {statistic}"));
                        return null;
                }
            }
            catch(NullReferenceException)
            {
                Program.Log(new LogMessage(LogSeverity.Verbose, "ProfileService.cs", $"PlayerData or GuildMember not found | Stat: {statistic}"));
                return null;
            }
        }
        // default profile structure (when newly created)

        // calling a profile will return a boolean value
        public async static Task<MainProfileResult> InvokeMainProfileAsync(string mainProfileId, string uuid, GuildData guildData = null)
        {
            // invokes the main profile can gives a boolean data / results
            MainProfile mp = await ProfileService.LoadMainProfileAsync(mainProfileId);
            PlayerData playerData = await Program.HypixelService.GetPlayerDataByUuidAsync(uuid);

            if(mp == null || playerData == null)
            {
                return null;
            }
            GuildMember guildMember = HypixelService.GetGuildMember(uuid, guildData); 

            MainProfileResult mpr = new();

            mpr.Name = mp.Name;
            mpr.Uuid = uuid;
            mpr.Success = false;

            int totalConditionPassed = 0;

            foreach(string id in mp.Conditions)
            {
                if(id[0] == 'c')
                {
                    // load condition that returns true or false
                    ConditionProfileResult cpr = await ProfileService.InvokeConditionProfileAsync(id, uuid, playerData, guildMember);
                    mpr.conditionResults[id] = cpr;
                    if(cpr.Success == true)
                    {
                        totalConditionPassed++;
                    }
                }
                else if(id[0] == 'g')
                {
                    // load group that loads more groups or conditions
                    GroupProfileResult gpr = await ProfileService.InvokeGroupProfileAsync(id, uuid, playerData, guildMember);
                    mpr.groupResults[id] = gpr;
                    if(gpr.Success == true)
                    {
                        totalConditionPassed++;
                    }
                }
            }

            mpr.Success = totalConditionPassed == mp.Conditions.Count;
            return mpr;
        }

        public async static Task<GroupProfileResult> InvokeGroupProfileAsync(string groupProfileId, string uuid, PlayerData playerData, GuildMember? guildMember)
        {
            GroupProfile gp = await ProfileService.LoadGroupProfileAsync(groupProfileId);

            GroupProfileResult gpr = new GroupProfileResult();
            gpr.Name = gp.Name;
            gpr.Success = false;
            // condition + group Ids
            int totalConditionPassed = 0;
            foreach(string id in gp.Conditions)
            {
                if(id[0] == 'c')
                {
                    ConditionProfileResult cpr = await ProfileService.InvokeConditionProfileAsync(id, uuid, playerData, guildMember);
                    if(cpr.Success == true)
                    {
                        totalConditionPassed++;
                        gpr.conditionResults[id] = cpr;
                    }
                }
                else if(id[0] == 'g')
                {
                    if(id.Substring(1, 3) == groupProfileId.Substring(1, 3))
                    {
                        await Program.Log(new LogMessage(LogSeverity.Error, "ProfileService.cs", "Group profile should not reference itself")); 
                        // maybe implement system for two group profiles referencing each other
                        continue;
                    }
                    GroupProfileResult gpr_ = await ProfileService.InvokeGroupProfileAsync(id, uuid, playerData, guildMember);
                    if(gpr_.Success == true)
                    {
                        totalConditionPassed++;
                        gpr.groupResults[id] = gpr_;
                    }
                }
            }
            
            gpr.Success = totalConditionPassed >= gp.TotalConditionsPass;
            return gpr;
        }

        public async static Task<ConditionProfileResult> InvokeConditionProfileAsync(string conditionProfileId, string uuid, PlayerData playerData, GuildMember? guildMember)
        {
            ConditionProfile cp = await ProfileService.LoadConditionProfileAsync(conditionProfileId);
            
            ConditionProfileResult cpr = new ConditionProfileResult();

            double? statisticValue = ProfileService.GetStatistic(cp.Statistic, uuid, playerData, guildMember);
            bool? isConditionPassed = ProfileService.CheckCondition(cp.Operator, statisticValue, cp.Value);

            cpr.Name = cp.Name;
            cpr.Statistic = cp.Statistic;
            cpr.Success = isConditionPassed;
            cpr.Value = statisticValue;

            return cpr;
        }
    }
}