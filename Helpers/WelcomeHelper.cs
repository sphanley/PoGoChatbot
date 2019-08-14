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
                        MessageFactory.Text($"Welcome, {addedMemberName}! We're always excited to have a new trainer join our community! I've got a few helpful tips to help you get started."),
                        MessageFactory.Text(Constants.WelcomeMessages.FirstTimeNameFormatMessage),
                        MessageFactory.Text(Constants.WelcomeMessages.FirstTimeBotTutorialMessage)
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
