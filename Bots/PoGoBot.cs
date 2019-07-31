// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with EchoBot .NET Template version v4.5.1

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace PoGoChatbot.Bots
{
    public class PoGoBot : ActivityHandler
    {
        //protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        //{
        //}

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Welcome, {member.Name}! We're always excited to have a new trainer join our community!"), cancellationToken);
                    await turnContext.SendActivityAsync(MessageFactory.Text($"I've got a few helpful resources to help you get started. To start, here's a map of the gyms where we typically raid: https://tinyurl.com/y3rddyjd"), cancellationToken);
                }
            }
        }
    }
}
