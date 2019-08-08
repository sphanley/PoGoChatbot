using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace PoGoChatbot.Helpers
{
    public static class InvocationHelper
    {
        public static async Task HandleInvocationActivity(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            switch (turnContext.Activity.Text.Split(" ")[0])
            {
                case "!map":
                    await HandleMapInvocation(turnContext, cancellationToken);
                    break;
                case "!whereis":
                    await HandleWhereIsInvocation(turnContext, cancellationToken);
                    break;
                case "!help":
                    await HandleHelpInvocation(turnContext, cancellationToken);
                    break;
            }
        }

        private static async Task HandleHelpInvocation(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text(
                "• For the map of all gyms within this group's area, say \"!map\".\n\n" +
                "• For the location of a specific gym, say \"!whereis {{Gym Name}}\" - for example, \"!whereis Spirit Corner\" or \"!whereis Coventry Arch\".\n\n" +
                "• For this list, say \"!help\"."
            ), cancellationToken);
        }

        private static async Task HandleMapInvocation(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text(Constants.MapMessage), cancellationToken);
        }

        private static async Task HandleWhereIsInvocation(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            Regex whereIsRegex = new Regex($"^!whereis \"?([^\"]+)\"?$");
            Match gymNameMatch = whereIsRegex.Match(turnContext.Activity.Text);
            var searchTerm = (gymNameMatch.Success && gymNameMatch.Groups.Count >= 2) ? gymNameMatch.Groups[1].Value : "";

            if (string.IsNullOrEmpty(searchTerm))
            {
                await turnContext.SendActivityAsync(MessageFactory.Text(Constants.WhereisNoGymNameMessage), cancellationToken);
            }
            else
            {
                var gymMatches = GymLookupHelper.SearchForGyms(searchTerm);

                if (gymMatches.Count == 0)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Sorry, but I couldn't find a gym called {searchTerm}."), cancellationToken);
                }
                else
                {
                    if (gymMatches.Count > 1)
                    {
                        await turnContext.SendActivityAsync(MessageFactory.Text($"I found more than one gym with names similar to {searchTerm}:"), cancellationToken);
                    }
                    foreach (var gym in gymMatches.Take(3))
                    {
                        var messageText = $"Here's the location of {gym.Name}.";
                        if (gym.IsEXEligible) messageText += " It's an EX Raid eligible gym!";
                        messageText += $" https://www.google.com/maps/search/?api=1&query={gym.Location.Latitude},{gym.Location.Longitude}";

                        var textMsg = MessageFactory.Text(messageText);

                        await turnContext.SendActivityAsync(textMsg, cancellationToken);

                        // TODO: update below code if Microsoft adequately resolves https://github.com/microsoft/BotFramework-Services/issues/101
                        //var attachmentMsg = MessageFactory.Attachment(new Attachment("image/png", "https://i.imgur.com/vta1imR.png"));
                        //var channelData = new GroupMeChannelData
                        //{ 
                        //    Attachment = new GroupMeLocationAttachment
                        //    {
                        //        Latitude = gym.Location.Latitude,
                        //        Longitude = gym.Location.Longitude,
                        //        Name = gym.Name
                        //    }
                        //};
                        //attachmentMsg.ChannelData = channelData;

                        //await turnContext.SendActivitiesAsync(new[] { textMsg, attachmentMsg }, cancellationToken);

                    }
                    if (gymMatches.Count > 1)
                    {
                        await turnContext.SendActivityAsync(MessageFactory.Text($"If I didn't find what you were looking for, feel free to try again with a different search term."), cancellationToken);
                    }
                }

            }
        }
    }
}
