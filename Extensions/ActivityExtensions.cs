using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;
using PoGoChatbot.Models;

namespace PoGoChatbot
{
    public static class ActivityExtensions
    {
        public static string getAddedGroupMeMemberName(this IMessageActivity activity)
        {
            if (activity.From.Id != "system" && activity.From.Name != "GroupMe") return null;

            Regex joinedRegex = new Regex($"^(.+) has joined the group.$");
            Regex addedRegex = new Regex($"^.+ added (.+) to the group.");

            Match joinedMatch = joinedRegex.Match(activity.Text);
            if (joinedMatch.Success && joinedMatch.Groups.Count >= 2) return joinedMatch.Groups[1].Value;

            Match addedMatch = addedRegex.Match(activity.Text);
            if (addedMatch.Success && addedMatch.Groups.Count >= 2) return addedMatch.Groups[1].Value;
            
            return null;
        }

        public static bool IsCreatedPoll(this IMessageActivity activity)
        {
            JArray channelData;
            return (activity.TryGetChannelData(out channelData) && channelData.Any(token => token.ToObject<GroupMeAttachment>().Type == "poll"));                
        }
    }
}
