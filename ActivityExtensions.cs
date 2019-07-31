using System.Text.RegularExpressions;
using Microsoft.Bot.Schema;

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
    }
}
