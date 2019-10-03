using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GroupMeExtensions.Models;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;

namespace GroupMeExtensions
{
    public static class ActivityExtensions
    {
        public static IList<ChannelAccount> NewMembers(this IMessageActivity activity)
        {
            var addedMembers = new List<ChannelAccount>();
            if (activity.From.Id != "system" || activity.From.Name != "GroupMe")
            {
                return addedMembers;
            }

            Regex joinedRegex = new Regex($"^(.+) has joined the group$");
            Regex addedRegex = new Regex($"^.+ added (.+) to the group.$");

            Match joinedMatch = joinedRegex.Match(activity.Text);
            if (joinedMatch.Success && joinedMatch.Groups.Count >= 2)
            {
                addedMembers.Add(new ChannelAccount { Name = joinedMatch.Groups[1].Value });
            }

            Match addedMatch = addedRegex.Match(activity.Text);
            if (addedMatch.Success && addedMatch.Groups.Count >= 2)
            {
                addedMembers.Add(new ChannelAccount { Name = addedMatch.Groups[1].Value });
            }

            return addedMembers;
        }

        public static IList<ChannelAccount> ReturningMembers(this IMessageActivity activity)
        {
            var returningMembers = new List<ChannelAccount>();
            if (activity.From.Id != "system" || activity.From.Name != "GroupMe")
            {
                return returningMembers;
            }

            Regex rejoinedRegex = new Regex($"^(.+) has rejoined the group$");

            Match rejoinedMatch = rejoinedRegex.Match(activity.Text);
            if (rejoinedMatch.Success && rejoinedMatch.Groups.Count >= 2)
            {
                returningMembers.Add(new ChannelAccount { Name = rejoinedMatch.Groups[1].Value });
            }

            return returningMembers;
        }

        public static IList<GroupMeAttachment> Polls(this IMessageActivity activity)
        {
            if (activity.TryGetChannelData(out JArray channelData))
            {
                return channelData.Select(data => data.ToObject<GroupMeAttachment>())
                                  .Where(attachment => attachment.Type == "poll")
                                  .ToList();
            }
            return new List<GroupMeAttachment>();

        }
    }
}
