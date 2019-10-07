namespace PoGoChatbot
{
    public static class Constants
    {
        public const string WhereisNoGymNameMessage = "Sorry, but you need to invoke \"!whereis\" with a gym name! For example, you can say \"!whereis Melt\" or \"!whereis Bird Friendly Habitat\".";

        public const string VoteAndLikeReminder = "Remember, if you're planning on attending this raid, please make sure that in addition to voting, you also \"like\" the poll using the heart button in the corner. " +
            "These \"likes\" are the only way that people can see who specifically is planning to attend, and ensure everyone is present for the start!";

        public const string RaidBossesMessage = "Thanks to the hard work of the Silph Research Group, a list of the known current raid bosses can be found at https://thesilphroad.com/raid-bosses";

        public static class Numeric
        {
            public const decimal DOUBLE_RESISTANT = 0.390625m;
            public const decimal RESISTANT = 0.625m;
            public const decimal WEAK = 1.6m;
            public const decimal DOUBLE_WEAK = 2.56m;
        }

        public static class WelcomeMessages
        {
            public const string FirstTimeNameFormatMessage = "I've got a few important tips to help you get started. First, if you haven't already, you'll want to use the \"Change Nickname\" option from the settings menu to set your name to this format: " +
                            "\"Name {TrainerName} {Team} {Level}\". For example, I was created by Sam (sphanley) Valor 40.\n\n" +
                            "Using this format helps us recognize each other, and estimate how many people are needed for a raid.";

            public const string ParameterizedFirstTimeBotTutorialMessage = "I can also provide lots of helpful info! For example, to see a map of the gyms where we raid, you can say \"!map\"," +
                        " or you can say things like \"!whereis {0}\" or \"!type Pikachu\". For a full list of possible commands and usage guidance, say \"!help\". And remember, have fun!";

            public const string WelcomeBackReminderMessage = "As a reminder, please make sure your name is in the format \"Name {TrainerName} {Team} {Level}\". And if you need a refresher on the ways I can provide helpful info, just say \"!help\". And have fun!"; 
        }
    }
}
