using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using GroupMeExtensions.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;
using PoGoChatbot.Extensions;

namespace PoGoChatbot.Helpers
{
    public static class InvocationHelper
    {
        private static readonly HttpClient httpClient = new HttpClient { BaseAddress = new Uri("https://api.groupme.com/") };

        public static async Task HandleInvocationActivity(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            switch (turnContext.Activity.Text.Split(" ")[0].ToLowerInvariant())
            {
                case "!bugreport":
                case "!featurerequest":
                    await HandleBugReportInvocation(turnContext, cancellationToken);
                    break;
                case "!bosses":
                case "!raidbosses":
                    await HandleRaidBossesInvocation(turnContext, cancellationToken);
                    break;
                case "!map":
                    await HandleMapInvocation(turnContext, cancellationToken);
                    break;
                case "!type":
                    await HandleTypeLookupInvocation(turnContext, cancellationToken);
                    break;
                case "!whereis":
                    await HandleWhereIsInvocation(turnContext, cancellationToken);
                    break;
                case "!help":
                    await HandleHelpInvocation(turnContext, cancellationToken);
                    break;
            }
        }

        #region Invocation Handler Methods
        private static async Task HandleBugReportInvocation(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text($"To report a simple issue or feature idea, feel free to ping @Sam (sphanley) Valor 40 in the chat. " +
                $"If you're reporting something more complex or lengthy, you can send an email to {Environment.GetEnvironmentVariable("AdminEmailAddress")}, " +
                $"or if you are a power user and are comfortable opening a GitHub issue, you can do so here: https://github.com/sphanley/PoGoChatbot/issues/new"), cancellationToken);
        }


        private static async Task HandleHelpInvocation(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var exampleGymNames = VariableResources.GymNameExamples;
            await turnContext.SendActivityAsync(MessageFactory.Text(
                "• For the map of all gyms within this group's area, say \"!map\".\n\n" +
                $"• For the location of a specific gym, say \"!whereis {{Gym Name}}\" - for example, \"!whereis {exampleGymNames[0]}\" or \"!whereis {exampleGymNames[1]}\".\n\n" +
                "• For the type(s), strengths and weaknesses of a specific pokemon, say \"!type {Pokemon Name}\" or \"!type {Pokemon number}\"  - for example, \"!type Pikachu\" or \"!type 25\".\n\n" +
                "• For a link to the list of known current raid bosses, say \"!raidbosses\" or \"!bosses\".\n\n" +
                "• For info on how to send a bug report or feature request, say \"!bugreport\" or \"!featurerequest\".\n\n" +
                "• For this list, say \"!help\"."
            ), cancellationToken);
        }

        private static async Task HandleMapInvocation(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            Regex mapRegex = new Regex($"^!map \"?([^\"]+)\"?$", RegexOptions.IgnoreCase);
            var argument = mapRegex.Match(turnContext.Activity.Text).Groups?.ElementAtOrDefault(1)?.Value ?? string.Empty;
            var normalizedGroupName = VariableResources.ValidateAndNormalizeGroupName(argument);
            var mapUrl = VariableResources.GetMapUrl(normalizedGroupName);

            if (!string.IsNullOrEmpty(mapUrl))
            {
                var messageText = $"Here's a map of the gyms where ";
                bool argumentIsDifferentGroup = !string.IsNullOrEmpty(normalizedGroupName) && normalizedGroupName != VariableResources.GroupName;
                messageText += argumentIsDifferentGroup ? $"the {normalizedGroupName} group typically raids: " : "we  typically raid: ";

                await turnContext.SendActivityAsync(MessageFactory.Text($"{messageText} {mapUrl}"), cancellationToken);
            }
            else
            {
                await turnContext.SendActivityAsync(MessageFactory.Text($"Sorry, I couldn't find a group matching the name \"{normalizedGroupName}\". The supported groups are Near East Side, Shaker Heights, and University Circle"));
            }
        }

        private static async Task HandleRaidBossesInvocation(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text(Constants.RaidBossesMessage), cancellationToken);

            var tierFiveRaids = await PokeBattlerApiHelper.GetRaids(tier: 5);
            if (tierFiveRaids.All(raid => !string.IsNullOrEmpty(raid.Article?.InfographicURL)))
            {
                bool pluralize = tierFiveRaids.Count() > 1;

                await turnContext.SendActivityAsync(MessageFactory.Text($"Here's {(pluralize ? "infographics" : " an infographic")} with details about the current tier-five raid {(pluralize ? "bosses" : "boss")}, courtesy of PokeBattler."));

                await turnContext.SendActivitiesAsync(
                    tierFiveRaids
                    .Select(raid =>
                        MessageFactory.Attachment(new Attachment("image/png", raid.Article.InfographicURL)) as IActivity
                    )
                    .ToArray()
                );
            }
        }

        private static async Task HandleTypeLookupInvocation(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            Regex typeLookupRegex = new Regex($"^!type \"?([^\"]+)\"?$", RegexOptions.IgnoreCase);
            Match typeLookupMatch = typeLookupRegex.Match(turnContext.Activity.Text);
            var searchTerm = (typeLookupMatch.Success && typeLookupMatch.Groups.Count >= 2) ? typeLookupMatch.Groups[1].Value : "";

            var pokemon = await PoGoApiHelper.GetPokemonType(searchTerm);
            if (pokemon != null)
            {
                var typeOrTypes = pokemon.Type.Count == 1 ? "type" : "types";
                var messageText = $"{pokemon.Name} has the {typeOrTypes} {string.Join(" and ", pokemon.Type)}. That means it is:\n\n";

                var doubleResistantAgainst = pokemon.MatchupsForType.Where(pair => pair.Value == Constants.Numeric.DOUBLE_RESISTANT).Select(pair => pair.Key);
                if (doubleResistantAgainst.Any()) messageText += $"• Double resistant against {doubleResistantAgainst.CommaSeparateWithAnd()}.\n\n";

                var resistantAgainst = pokemon.MatchupsForType.Where(pair => pair.Value == Constants.Numeric.RESISTANT).Select(pair => pair.Key);
                if (resistantAgainst.Any()) messageText += $"• Resistant against {resistantAgainst.CommaSeparateWithAnd()}.\n\n";

                var weakAgainst = pokemon.MatchupsForType.Where(pair => pair.Value == Constants.Numeric.WEAK).Select(pair => pair.Key);
                if (weakAgainst.Any()) messageText += $"• Weak against {weakAgainst.CommaSeparateWithAnd()}.\n\n";

                var doubleWeakAgainst = pokemon.MatchupsForType.Where(pair => pair.Value == Constants.Numeric.DOUBLE_WEAK).Select(pair => pair.Key);
                if (doubleWeakAgainst.Any()) messageText += $"• Double weak against {doubleWeakAgainst.CommaSeparateWithAnd()}.\n\n";

                await turnContext.SendActivityAsync(MessageFactory.Text(messageText));
            }
            else
            {
                await turnContext.SendActivityAsync(MessageFactory.Text($"Sorry, I couldn't find a pokemon with the name or number \"{searchTerm}\"."), cancellationToken);
            }
        }

        private static async Task HandleWhereIsInvocation(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            Regex whereIsRegex = new Regex($"^!whereis \"?([^\"]+)\"?$", RegexOptions.IgnoreCase);
            Match gymNameMatch = whereIsRegex.Match(turnContext.Activity.Text);
            var searchTerm = (gymNameMatch.Success && gymNameMatch.Groups.Count >= 2) ? gymNameMatch.Groups[1].Value : "";

            if (string.IsNullOrEmpty(searchTerm))
            {
                await turnContext.SendActivityAsync(MessageFactory.Text(Constants.WhereisNoGymNameMessage), cancellationToken);
            }
            else
            {
                var gymMatches = GymLookupHelper.SearchForGyms(searchTerm, VariableResources.GroupName);
                if (gymMatches.Any())
                {
                    if (gymMatches.Count > 1)
                    {
                        await turnContext.SendActivityAsync(MessageFactory.Text($"I found more than one gym with names similar to \"{searchTerm}\":"), cancellationToken);
                    }
                    foreach (var gym in gymMatches.Take(3))
                    {
                        var messageText = $"Here's the location of {gym.Name}.";
                        if (gym.Territory.Any(groupName => !groupName.Equals(VariableResources.GroupName)))
                        {
                            messageText += gym.Territory.Any(name => name.Equals(VariableResources.GroupName)) ?
                                " It's " :
                                " It's outside this group's territory, ";
                            messageText += $"in an area where the {gym.Territory.CommaSeparateWithAnd()} {(gym.Territory.Count() == 1 ? "group raids" : "groups may raid")}.";
                        }
                        if (gym.IsEXEligible) messageText += " It's an EX Raid eligible gym!";

                        #region - This is a temporary hack to send the Location attachment while waiting for a fix for https://github.com/microsoft/BotFramework-Services/issues/101
                        var botId = VariableResources.GroupMeBotId;
                        var message = JObject.FromObject(new
                        {
                            text = messageText,
                            bot_id = botId,
                            attachments = new[] {
                                new GroupMeLocationAttachment
                                {
                                    Latitude = gym.Location.Latitude,
                                    Longitude = gym.Location.Longitude,
                                    Name = gym.Name
                                }
                            }
                        });

                        if (!string.IsNullOrEmpty(botId))
                        {
                            var messageJson = new StringContent(message.ToString(), Encoding.UTF8, "application/json");
                            _ = httpClient.PostAsync("v3/bots/post", messageJson);
                        }
                        else if (turnContext.Activity.ChannelId == "emulator")
                        {

                            await turnContext.SendActivityAsync(MessageFactory.Text("Unable to send attachment via emulator. Message content below:"));
                            await turnContext.SendActivityAsync(MessageFactory.Text(message.ToString()));
                        }
                        #endregion
                        if (!string.IsNullOrEmpty(gym.Description))
                        {
                            await turnContext.SendActivityAsync(MessageFactory.Text(gym.Description), cancellationToken);
                        }
                    }
                    if (gymMatches.Count > 1)
                    {
                        await turnContext.SendActivityAsync(MessageFactory.Text($"If I didn't find what you were looking for, feel free to try again with a different search term."), cancellationToken);
                    }
                }
                else
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Sorry, but I couldn't find a gym called \"{searchTerm}\"."), cancellationToken);
                }
            }
        }

        #endregion
    }
}
