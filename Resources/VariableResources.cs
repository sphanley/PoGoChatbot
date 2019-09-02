using System;

namespace PoGoChatbot
{
    public static class VariableResources
    {
        // These const variables can be used to easily set default values for local debugging without modifying environment variables
        private const string DefaultBotId = "";
        private const string DefaultGroupName = "Near East Side";
        private const string DefaultGymNameExamples = "PLACEHOLDER_1,PLACEHOLDER_2";
        private const string DefaultMapUrl = "http://bit.ly/nesraidmap";
        private const string DefaultWelcomePacketUrl = "http://bit.ly/nespogoinfo";

        // These public variables should have their associated environment variables set on any deployment environment
        public static readonly string GroupMeBotId = Environment.GetEnvironmentVariable("GroupMeBotId") ?? DefaultBotId;
        public static readonly string GroupName = Environment.GetEnvironmentVariable("GroupMeGroupName") ?? DefaultGroupName;
        public static readonly string[] GymNameExamples = (Environment.GetEnvironmentVariable("GymNameExamples") ?? DefaultGymNameExamples).Split(',');
        public static readonly string MapUrl = Environment.GetEnvironmentVariable("MapUrl") ?? DefaultMapUrl;
        public static readonly string WelcomePacketUrl = Environment.GetEnvironmentVariable("WelcomePacketUrl") ?? DefaultWelcomePacketUrl;
    }
}
