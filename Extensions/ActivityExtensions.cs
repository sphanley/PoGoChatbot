using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;
using PoGoChatbot.Models;

namespace PoGoChatbot
{
    public static class ActivityExtensions
    {
        public static bool TryGetAddedGroupMeMemberName(this IMessageActivity activity, out string addedMemberName)
        {
            if (activity.From.Id != "system" || activity.From.Name != "GroupMe")
            {
                addedMemberName = null;
                return false;
            }

            Regex joinedRegex = new Regex($"^(.+) has joined the group$");
            Regex addedRegex = new Regex($"^.+ added (.+) to the group.$");

            Match joinedMatch = joinedRegex.Match(activity.Text);
            if (joinedMatch.Success && joinedMatch.Groups.Count >= 2)
            {
                addedMemberName = joinedMatch.Groups[1].Value;
                return true;
            }

            Match addedMatch = addedRegex.Match(activity.Text);
            if (addedMatch.Success && addedMatch.Groups.Count >= 2)
            {
                addedMemberName = addedMatch.Groups[1].Value;
                return true;
            }

            addedMemberName = null;
            return false;
        }

        public static bool TryGetReturningGroupMeMemberName(this IMessageActivity activity, out string returningMemberName)
        {
            if (activity.From.Id != "system" || activity.From.Name != "GroupMe")
            {
                returningMemberName = null;
                return false;
            }

            Regex rejoinedRegex = new Regex($"^(.+) has rejoined the group$");

            Match rejoinedMatch = rejoinedRegex.Match(activity.Text);
            if (rejoinedMatch.Success && rejoinedMatch.Groups.Count >= 2)
            {
                returningMemberName = rejoinedMatch.Groups[1].Value;
                return true;
            }

            returningMemberName = null;
            return false;
        }

        public static bool IsCreatedPoll(this IMessageActivity activity)
        {
            return (activity.TryGetChannelData(out JArray channelData) && channelData.Any(token => token.ToObject<GroupMeAttachment>().Type == "poll"));
        }

        public static void SetGroupNameFromConversationId(this IActivity activity)
        {
            activity.Conversation.Name = VariableResources.GroupName;
        }
    }
}
