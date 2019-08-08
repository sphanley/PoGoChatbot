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
                        MessageFactory.Text($"Welcome, {addedMemberName}! We're always excited to have a new trainer join our community! I've got a few helpful tips and to help you get started."),
                        MessageFactory.Text($"To start, if you haven't already, you'll want to use the \"Change Nickname\" option from the setting menu to change your name to use our standard format, \"Name {{TrainerName}} {{Team}} {{Level}}\".\n\n" +
                            "For example, I'm a bot, and I was created by @Sam (sphanley) Valor 40.\n\n" +
                            "Using this format helps us recognize each other, and estimate how many people are needed for a raid!"),
                        MessageFactory.Text("Next, here's a map of the gyms where we typically raid: https://tinyurl.com/y3rddyjd. If you need this link later, just say \"!map\"."),
                        MessageFactory.Text("I can also provide on-demand information! For example, you can say \"!whereis {{Gym Name}}\" to get the location of a gym. For more commands and usage guidance, say \"!help\". And have fun!")
                    }, cancellationToken);
        }
    }
}
