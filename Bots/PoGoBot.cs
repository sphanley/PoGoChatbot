﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DuoVia.FuzzyStrings;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using PoGoChatbot.Models;

namespace PoGoChatbot.Bots
{
    public class PoGoBot : ActivityHandler
    {
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            // This is a temporary hack to work around the fact that GroupMe doesn't correctly route add/join messages through OnMembersAddedAsync

            var addedMemberName = turnContext.Activity.getAddedGroupMeMemberName();

            if (!String.IsNullOrEmpty(addedMemberName))
            {
                await turnContext.SendActivityAsync(MessageFactory.Text($"Welcome, {addedMemberName}! We're always excited to have a new trainer join our community!"), cancellationToken);
                await turnContext.SendActivityAsync(MessageFactory.Text($"I've got a few helpful resources to help you get started. To start, here's a map of the gyms where we typically raid: https://tinyurl.com/y3rddyjd. If you need this link later, just say \"!map\"."), cancellationToken);
            }

            if (turnContext.Activity.Text.StartsWith("!", StringComparison.Ordinal))
            {
                await HandleInvocationActivity(turnContext, cancellationToken);
            }
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Welcome, {member.Name}! We're always excited to have a new trainer join our community!"), cancellationToken);
                    await turnContext.SendActivityAsync(MessageFactory.Text($"I've got a few helpful resources to help you get started. To start, here's a map of the gyms where we typically raid: https://tinyurl.com/y3rddyjd. If you need this link later, just say \"!map\"."), cancellationToken);
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
                                var msg = MessageFactory.Text(messageText);

                                await turnContext.SendActivityAsync(msg);
                            }
                            if (gymMatches.Count > 1)
                            {
                                await turnContext.SendActivityAsync(MessageFactory.Text($"If I didn't find what you were looking for, feel free to try again with a different search term."), cancellationToken);
                            }
                        }

                    }
                    break;
            }
        }
    }
}
