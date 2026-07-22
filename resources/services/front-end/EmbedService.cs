using Discord;
using SovereigntyBot.Services.Endpoints.Cache;
using SovereigntyBot.Services.Endpoints.Profile;

namespace SovereigntyBot.Services
{
    public class EmbedService
    {
        public static EmbedBuilder Error(string errorMessage)
        {
            return new EmbedBuilder()
                .WithAuthor("ERROR")
                .WithColor(Color.Red)
                .WithDescription(errorMessage);
        }

        public static EmbedBuilder Success(string successMessage)
        {
            return new EmbedBuilder()
                .WithAuthor("Success")
                .WithColor(Color.Green)
                .WithDescription(successMessage)
                .AddField("cool", "cool", false);
        }

        public static EmbedBuilder StatEmbed(string username, bool inGuild, MainProfileResult result)
        {
            // username (DONE)
            // eligibility (DONE)
            // in guild / not in guild (DONE)
            
            // Each field contains a group
            // separate conditions will be placed in "Others"

            bool eligibility = result.Success;

            // iterate through each groupresults 
            // create new embed
            foreach(GroupProfileResult gResult in result.groupResults.Values)
            {
                var field = new EmbedFieldBuilder();
                // dive deep until you get the groups that only have conditions
                // if there are separate conditions, store them to put in "Others" field
            }
            
            // check if there are condition results 
            // if got, iterate through them and put in an "Others" field
            return new EmbedBuilder();
        }

        // private static List<GroupProfileResult> Search(List<GroupProfileResult> gprs, ref List<ConditionProfileResult> others)
        // {
        //     // split into groups with only conditions and a group of all "others" conditions

        //     // iterate through gprs
        //     foreach(GroupProfileResult gpr in gprs)
        //     {
        //         var gprs2 = gpr.groupResults.Values; // is gprs2 null if empty?
        //         if(gprs2.Count() > 0 )
        //         {
        //             var cprs = gpr.conditionResults.Values;
        //             if(cprs.Count() > 0)
        //             {
        //                 others.AddRange(cprs);
        //             }
        //             Search(gprs2.ToList(), ref others);
        //         }
        //     }
        //     return gprs
        // }

        public static MessageComponentData ModifyEmbedWithTable(EmbedBuilder wEmbed, Table table, ComponentBuilder wComponent = null)
        {
            wEmbed.Fields = table.GetDisplayFields();

            var mcd = new MessageComponentData();
            mcd.wEmbed = wEmbed;
            mcd.table = table;

            if(wComponent != null)
            {
                mcd.wComponent = wComponent.WithButton(table.GetControlPanelButton());
            }

            return mcd;
        }
    }
}