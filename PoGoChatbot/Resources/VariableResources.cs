using System;
using System.Collections.Generic;
using System.Linq;

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
        public static readonly List<string> GymNameExamples = (Environment.GetEnvironmentVariable("GymNameExamples") ?? DefaultGymNameExamples).Split(',').ToList();
        public static readonly string WelcomePacketUrl = GetWelcomePacketUrl();

        public static string GetMapUrl(string groupName)
        {
            if (string.IsNullOrEmpty(groupName)) groupName = GroupName;

            switch (groupName)
            {
                case "Near East Side":
                    return "https://bit.ly/nesraidmap";
                case "University Circle":
                    return "https://bit.ly/ucraidmap";
                case "Shaker Heights":
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

        public static string ValidateAndNormalizeGroupName(string groupName)
        {
            switch (groupName.ToLowerInvariant())
            {
                case "near east side":
                case "near eastside":
                    return "Near East Side";
                case "university circle":
                case "uc":
                    return "University Circle";
                case "shaker heights":
                case "shaker":
                    return "Shaker Heights";
                default:
                    return string.Empty;
            }
        }
    }
}
