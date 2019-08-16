using Microsoft.Bot.Schema;

namespace PoGoChatbot
{
    public static class VariableResources
    {
        public static string GetMapUrl(IActivity activity)
        {
            switch (activity.Conversation.Id)
            {
                case "32638346":
                case "51947472":
                    return "http://bit.ly/neareastsidemap";
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
                default:
                    return "";
            }
        }

        public static string getWelcomePacketUrl(IActivity activity)
        {
            switch (activity.Conversation.Id)
            {
                case "32638346":
                case "51947472": // 51947472 is lab group
                    return "http://bit.ly/neareastsidefaq";
                default:
                    return "";
            }
        }
    }
}
