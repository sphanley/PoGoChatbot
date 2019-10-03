using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace PoGoChatbot.Helpers
{
    public static class WelcomeHelper
    {
        public static async Task SendWelcomeMessage(ChannelAccount member, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            if (member.Id != turnContext.Activity.Recipient.Id)
            {
                await turnContext.SendActivitiesAsync(new[] {
                    MessageFactory.Text($"Welcome, {member.Name}! We're always excited to have a new trainer join our community! Our group guidelines and FAQs can be found here: {VariableResources.WelcomePacketUrl}"),
                    MessageFactory.Text(Constants.WelcomeMessages.FirstTimeNameFormatMessage),
                    MessageFactory.Text(string.Format(Constants.WelcomeMessages.ParameterizedFirstTimeBotTutorialMessage, VariableResources.GymNameExamples.First()))
                }, cancellationToken); ;
            }
        }

        public static async Task SendWelcomeBackMessage(ChannelAccount member, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            if (member.Id != turnContext.Activity.Recipient.Id)
            {
                await turnContext.SendActivitiesAsync(new[] {
                    MessageFactory.Text($"Welcome back to the group, {member.Name}! I'm glad to see you again."),
                    MessageFactory.Text(Constants.WelcomeMessages.WelcomeBackReminderMessage)
                }, cancellationToken);
            }
        }
    }
}
