using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using PoGoChatbot.Models;

namespace PoGoChatbot.Bots
{
    public class PoGoBot : ActivityHandler
    {
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            // This is a temporary hack to work around the fact that GroupMe doesn't correctly route add/join messages through OnMembersAddedAsync

            var addedMemberName = turnContext.Activity.getAddedGroupMeMemberName();

            if (!string.IsNullOrEmpty(addedMemberName))
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
            else if (turnContext.Activity.Text.StartsWith("!", StringComparison.Ordinal))
            {
                await HandleInvocationActivity(turnContext, cancellationToken);
            }
            else if (turnContext.Activity.IsCreatedPoll())
            {
                await turnContext.SendActivityAsync(MessageFactory.Text(Constants.VoteAndLikeReminder), cancellationToken);
            }
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivitiesAsync(new[] {
                        MessageFactory.Text($"Welcome, {member.Name}! We're always excited to have a new trainer join our community! I've got a few helpful tips and to help you get started."),
                        MessageFactory.Text($"To start, if you haven't already, you'll want to use the \"Change Nickname\" option from the settings menu to set your name to use our standard format, \"Name {{TrainerName}} {{Team}} {{Level}}\".\n\n" +
                            "For example, I'm a bot, and I was created by @Sam (sphanley) Valor 40.\n\n" +
                            "Using this format helps us recognize each other, and estimate how many people are needed for a raid!"),
                        MessageFactory.Text("Next, here's a map of the gyms where we typically raid: https://tinyurl.com/y3rddyjd. If you need this link later, just say \"!map\"."),
                        MessageFactory.Text("I can also provide on-demand information! For example, you can say \"!whereis {{Gym Name}}\" to get the location of a gym. For more commands and usage guidance, say \"!help\". And have fun!")
                    }, cancellationToken);
                }
            }
        }

        private async Task HandleInvocationActivity(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            // TODO: Figure out some less goofy/monolithic way to architecture this invocation switch logic
            switch (turnContext.Activity.Text.Split(" ")[0])
            {
                case "!map":
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Here's a map of the gyms where we typically raid: https://tinyurl.com/y3rddyjd."));
                    break;
                case "!whereis":
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
                            await turnContext.SendActivityAsync(MessageFactory.Text($"Sorry, but I couldn't find a gym called \"{searchTerm}\"."), cancellationToken);
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

                                // TODO: update below code if Microsoft acknowledges and/or fixes https://github.com/microsoft/BotFramework-Services/issues/101
                                var msg = MessageFactory.Text(messageText);
                                var channelData = new GroupMeChannelData()
                                {
                                    Attachments = new GroupMeAttachments
                                    {
                                        new GroupMeLocationAttachment()
                                        {
                                            Latitude = gym.Location.Latitude,
                                            Longitude = gym.Location.Longitude,
                                            Name = gym.Name
                                        }
                                    }                                
                                };
                                msg.ChannelData = channelData;

                                await turnContext.SendActivityAsync(msg);
                            }
                            if (gymMatches.Count > 1)
                            {
                                await turnContext.SendActivityAsync(MessageFactory.Text($"If I didn't find what you were looking for, feel free to try again with a different search term."), cancellationToken);
                            }
                        }

                    }
                    break;
                case "!help":
                    await turnContext.SendActivityAsync(MessageFactory.Text("• For the map of all gyms within this group's area, say \"!map\".\n\n"
                        + "• For the location of a specific gym, say \"!whereis {{Gym Name}}\" - for example, \"!whereis Spirit Corner\" or \"!whereis Coventry Arch\".\n\n"
                        + "• For this list, say \"!help\"."), cancellationToken);
                    break;
            }
        }
    }
}
