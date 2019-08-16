﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using PoGoChatbot.Helpers;

namespace PoGoChatbot.Bots
{
    public class PoGoBot : ActivityHandler
    {
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            // To use this mechanism to provide an easy reference to a friendly name for each group, set an Environment variable called "GroupNameMappings"
            //  containing key/value pairs for each chat where the bot is active, such that the key is the channel's Conversation ID, and the value is the friendly name, like:
            // "{ '12345678': 'group name', ... }"
            turnContext.Activity.SetGroupNameFromConversationId();

            // This first if statement is a temporary hack to work around the fact that GroupMe doesn't correctly route add/join messages through OnMembersAddedAsync.
            // Once https://github.com/microsoft/BotFramework-Services/issues/97 is resolved, welcome messages need only be handled by OnMembersAddedAsync
            if (turnContext.Activity.TryGetAddedGroupMeMemberName(out string addedMemberName))
            {
                await WelcomeHelper.SendWelcomeMessage(addedMemberName, turnContext, cancellationToken);
            }
            if (turnContext.Activity.TryGetReturningGroupMeMemberName(out string returningMemberName))
            {
                await WelcomeHelper.SendWelcomeBackMessage(returningMemberName, turnContext, cancellationToken);
            }
            else if (turnContext.Activity.Text.StartsWith("!", StringComparison.Ordinal))
            {
                await InvocationHelper.HandleInvocationActivity(turnContext, cancellationToken);
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
                    await WelcomeHelper.SendWelcomeMessage(member.Name, turnContext, cancellationToken);
                }
            }
        }
    }
}
