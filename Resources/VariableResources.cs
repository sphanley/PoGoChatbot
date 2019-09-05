using System;

namespace PoGoChatbot
{
    public static class VariableResources
    {
        // These const variables can be used to easily set default values for local debugging without modifying environment variables
        private const string DefaultBotId = "";
        private const string DefaultGroupName = "Near East Side";
        private const string DefaultGymNameExamples = "PLACEHOLDER_1,PLACEHOLDER_2";

        // These public variables should have their associated environment variables set on any deployment environment
        public static readonly string GroupMeBotId = Environment.GetEnvironmentVariable("GroupMeBotId") ?? DefaultBotId;
        public static readonly string GroupName = Environment.GetEnvironmentVariable("GroupMeGroupName") ?? DefaultGroupName;
        public static readonly string[] GymNameExamples = (Environment.GetEnvironmentVariable("GymNameExamples") ?? DefaultGymNameExamples).Split(',');
        public static readonly string WelcomePacketUrl = GetWelcomePacketUrl();

        public static string GetMapUrl(string groupName)
        {
            if (string.IsNullOrEmpty(groupName)) groupName = GroupName;

            switch (groupName.ToLowerInvariant())
            {
                case "near east side":
                case "near eastside":
                    return "https://bit.ly/nesraidmap";
                case "university circle":
                case "uc":
                    return "https://bit.ly/ucraidmap";
                case "shaker heights":
                case "shaker":
                    return "https://bit.ly/shakerheightsraidmap";
                default:
                    return string.Empty;
            }
        }

        private static string GetWelcomePacketUrl()
        {
            switch (GroupName)
            {
                case "Near East Side":
                    return "https://bit.ly/nespogoinfo";
                case "University Circle":
                    return "https://bit.ly/ucpogoinfo";
                case "Shaker Heights":
                    return "https://bit.ly/shakerheightspogoinfo";
                default:
                    return string.Empty;
            }
        }
    }
}
