using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using PoGoChatbot.Helpers;
using GroupMeExtensions;
using System.Linq;

namespace PoGoChatbot.Bots
{
    public class PoGoBot : ActivityHandler
    {
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            // This first if statement is a temporary hack to work around the fact that GroupMe doesn't correctly route add/join messages through OnMembersAddedAsync.
            // Once https://github.com/microsoft/BotFramework-Services/issues/97 is resolved, welcome messages need only be handled by OnMembersAddedAsync
            foreach(var member in turnContext.Activity.NewMembers())
            { 
				await WelcomeHelper.SendWelcomeMessage(member, turnContext, cancellationToken);
			}
			foreach(var member in turnContext.Activity.ReturningMembers())
            {
                await WelcomeHelper.SendWelcomeBackMessage(member, turnContext, cancellationToken);
            }
            if (turnContext.Activity.Text.StartsWith("!", StringComparison.Ordinal))
            {
                await InvocationHelper.HandleInvocationActivity(turnContext, cancellationToken);
            }
            if (turnContext.Activity.Polls().Any())
            {
                await turnContext.SendActivitiesAsync(new[] {
                    MessageFactory.Text(Constants.VoteAndLikeReminder),
                    MessageFactory.Text(Constants.MaskReminder)
                }, cancellationToken);
            }
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded) {
                await WelcomeHelper.SendWelcomeMessage(member, turnContext, cancellationToken);
            }
        }
    }
}
