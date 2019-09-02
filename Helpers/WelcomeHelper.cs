using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;

namespace PoGoChatbot.Helpers
{
    public static class WelcomeHelper
    {
        public static async Task SendWelcomeMessage(string addedMemberName, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivitiesAsync(new[] {
                        MessageFactory.Text($"Welcome, {addedMemberName}! We're always excited to have a new trainer join our community! Our group guidelines and FAQs can be found here: {VariableResources.WelcomePacketUrl}"),
                        MessageFactory.Text(Constants.WelcomeMessages.FirstTimeNameFormatMessage),
                        MessageFactory.Text(string.Format(Constants.WelcomeMessages.ParameterizedFirstTimeBotTutorialMessage, VariableResources.GymNameExamples.First()))
                    }, cancellationToken);
        }

        public static async Task SendWelcomeBackMessage(string returningMemberName, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivitiesAsync(new[] {
                MessageFactory.Text($"Welcome back to the group, {returningMemberName}! I'm glad to see you again."),
                MessageFactory.Text(Constants.WelcomeMessages.WelcomeBackReminderMessage)
                }, cancellationToken);
        }
    }
}
