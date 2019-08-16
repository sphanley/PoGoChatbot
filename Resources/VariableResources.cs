using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace PoGoChatbot
{
    public static class VariableResources
    {
        private static Dictionary<string, string> _botIds = new Dictionary<string, string>();

        public static string GetMapUrl(IActivity activity)
        {
            switch (activity.Conversation.Id)
            {
                case "32638346":
                case "51947472":
                    return "http://bit.ly/nesraidmap";
                case "31972760":
                    return "http://bit.ly/ucraidmap";
                default:
                    return "";
            }
        }

        public static string GetGroupName(IActivity activity)
        {
            switch (activity.Conversation.Id)
            {
                case "32638346":
                case "51947472": // 51947472 is lab group
                    return "Near East Side";
                case "31972760":
                    return "University Circle";
                default:
                    return "";
            }
        }

        public static string GetWelcomePacketUrl(IActivity activity)
        {
            switch (activity.Conversation.Id)
            {
                case "32638346":
                case "51947472": // 51947472 is lab group
                    return "http://bit.ly/nespogoinfo";
                case "31972760":
                    return "http://bit.ly/ucpogoinfo";
                default:
                    return "";
            }
        }

        public static string[] GetExampleGymNamesForGroup(IActivity activity)
        {
            switch (activity.Conversation.Id)
            {
                case "32638346":
                case "51947472": // 51947472 is lab group
                    return new[] { "Spirit Corner", "Bird Friendly Habitat" };
                case "31972760":
                    return new[] { "MOCA", "Batter Up" };
                default:
                    return new[] { "PLACEHOLDER_1", "PLACEHOLDER_2" };
            }
        }

        internal static string GetGroupMeBotId(IMessageActivity activity)
        {
            if (!_botIds.Any()) {
                var botIdMappings = Environment.GetEnvironmentVariable("GroupMeBotId");
                _botIds = JsonConvert.DeserializeObject<Dictionary<string, string>>(botIdMappings);
            }

            return _botIds.GetValueOrDefault(activity.Conversation.Id, "");
        }
    }
}
