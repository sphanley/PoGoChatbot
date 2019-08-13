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
                        MessageFactory.Text("To start, if you haven't already, you'll want to use the \"Change Nickname\" option from the settings menu to set your name to this format: " +
                            "\"Name {TrainerName} {Team} {Level}\". For example, I was created by @Sam (sphanley) Valor 39.\n\n" +
                            "Using this format helps us recognize each other, and estimate how many people are needed for a raid!"),
                        MessageFactory.Text("I can also provide information on demand! For example, to seee a map of the gyms where we raid, you can say \"!map\"," +
                        " or you can say things like \"!whereis Spirit Corner\" or \"!type Pikachu\". For a full list of possible commands, say \"!help\". And have fun!")
                    }, cancellationToken);
        }
    }
}
