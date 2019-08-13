namespace PoGoChatbot
{
    public static class Constants
    {
        public const string WhereisNoGymNameMessage = "Sorry, but you need to invoke \"!whereis\" with a gym name! For example, you can say \"!whereis Melt\" or \"!whereis Bird Friendly Habitat\".";

        public const string VoteAndLikeReminder = "Remember, if you're planning on attending this raid, please make sure that in addition to voting, you also \"like\" the poll using the heart button in the corner. " +
            "These \"likes\" are the only way that people can see who specifically is planning to attend, and ensure everyone is present for the start!";

        public const string MapMessage = "Here's a map of the gyms where we typically raid: https://tinyurl.com/y3rddyjd";

        public const string RaidBossesMessage = "Thanks to the hard work of the Silph Research Group, a list of the known current raid bosses can be found at https://thesilphroad.com/raid-bosses";

        public static class Numeric
        {
            public const decimal DOUBLE_RESISTANT = 0.390625M;
            public const decimal RESISTANT = 0.625M;
            public const decimal WEAK = 1.6M;
            public const decimal DOUBLE_WEAK = 2.56M;
        }
    }
}
