using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

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

            if(turnContext.Activity.Text.StartsWith("!", StringComparison.Ordinal))
            {
                await HandleInvocationActivity(turnContext);
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

        private async Task HandleInvocationActivity(ITurnContext<IMessageActivity> turnContext)
        {
            switch (turnContext.Activity.Text)
            {
                case "!map":
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Here's a map of the gyms where we typically raid: https://tinyurl.com/y3rddyjd."));
                    break;
                default:
                    break;
            }
        }
    }
}
