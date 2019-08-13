using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;
using PoGoChatbot.Extensions;
using PoGoChatbot.Models;

namespace PoGoChatbot.Helpers
{
    public static class InvocationHelper
    {
        private static readonly HttpClient httpClient = new HttpClient { BaseAddress = new Uri("https://api.groupme.com/") };

        public static async Task HandleInvocationActivity(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            switch (turnContext.Activity.Text.Split(" ")[0])
            {
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

        private static async Task HandleHelpInvocation(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text(
                "• For the map of all gyms within this group's area, say \"!map\".\n\n" +
                "• For the location of a specific gym, say \"!whereis {Gym Name}\" - for example, \"!whereis Spirit Corner\" or \"!whereis Coventry Arch\".\n\n" +
                "• For the type(s), strengths and weaknesses of a specific pokemon, say \"!type {Pokemon Name}\" or \"!type {Pokemon number}\"  - for example, \"!type Pikachu\" or \"!type 25\".\n\n" +
                "• For a link to the list of known current raid bosses , say \"!raidbosses\" or \"!bosses\".\n\n" +
                "• For this list, say \"!help\"."
            ), cancellationToken);
        }

        private static async Task HandleMapInvocation(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text(Constants.MapMessage), cancellationToken);
        }

        private static async Task HandleRaidBossesInvocation(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text(Constants.RaidBossesMessage), cancellationToken);
        }

        private static async Task HandleTypeLookupInvocation(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            Regex typeLookupRegex = new Regex($"^!type \"?([^\"]+)\"?$");
            Match typeLookupMatch = typeLookupRegex.Match(turnContext.Activity.Text);
            var searchTerm = (typeLookupMatch.Success && typeLookupMatch.Groups.Count >= 2) ? typeLookupMatch.Groups[1].Value : "";

            var pokemon = await PoGoApiHelper.GetPokemonType(searchTerm);
            if (pokemon != null)
            {
                var typeOrTypes = pokemon.Type.Length == 1 ? "type" : "types";
                var messageText = $"{pokemon.Name} has the {typeOrTypes} {string.Join(" and ", pokemon.Type)}. That means it is:\n\n";

                var doubleResistantAgainst = pokemon.MatchupsForType.Where(pair => pair.Value == Constants.Numeric.DOUBLE_RESISTANT).Select(pair => pair.Key);
                if(doubleResistantAgainst.Any()) messageText += $"• Double resistant against {doubleResistantAgainst.CommaSeparateWithAnd()}.\n\n";

                var resistantAgainst = pokemon.MatchupsForType.Where(pair => pair.Value == Constants.Numeric.RESISTANT).Select(pair => pair.Key);
                if(resistantAgainst.Any()) messageText += $"• Resistant against {resistantAgainst.CommaSeparateWithAnd()}.\n\n";

                var weakAgainst = pokemon.MatchupsForType.Where(pair => pair.Value == Constants.Numeric.RESISTANT).Select(pair => pair.Key);
                if(weakAgainst.Any()) messageText += $"• Weak against {weakAgainst.CommaSeparateWithAnd()}.\n\n";

                var doubleWeakAgainst = pokemon.MatchupsForType.Where(pair => pair.Value == Constants.Numeric.DOUBLE_RESISTANT).Select(pair => pair.Key);
                if(doubleWeakAgainst.Any()) messageText += $"• Double weak against {doubleWeakAgainst.CommaSeparateWithAnd()}.\n\n";

                await turnContext.SendActivityAsync(MessageFactory.Text(messageText));
            }
            else
            {
                await turnContext.SendActivityAsync(MessageFactory.Text($"Sorry, I couldn't find a pokemon with the name or number \"{searchTerm}\"."), cancellationToken);
            }
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
                if(gymMatches.Any())
                {
                    if (gymMatches.Count > 1)
                    {
                        await turnContext.SendActivityAsync(MessageFactory.Text($"I found more than one gym with names similar to \"{searchTerm}\":"), cancellationToken);
                    }
                    foreach (var gym in gymMatches.Take(3))
                    {
                        var messageText = $"Here's the location of {gym.Name}.";
                        if (gym.IsEXEligible) messageText += " It's an EX Raid eligible gym!";

                        #region - This is a temporary hack to send the Location attachment while waiting for a fix for https://github.com/microsoft/BotFramework-Services/issues/101
                        var message = JObject.FromObject(new
                        {
                            text = messageText,
                            bot_id = Environment.GetEnvironmentVariable("GroupMeBotId"),
                            attachments = new[] {
                                new GroupMeLocationAttachment
                                {
                                    Latitude = gym.Location.Latitude,
                                    Longitude = gym.Location.Longitude,
                                    Name = gym.Name
                                }
                            }
                        });

                        var messageJson = new StringContent(message.ToString(), Encoding.UTF8, "application/json");
                        _ = httpClient.PostAsync("v3/bots/post", messageJson);
                        #endregion
                        if(!string.IsNullOrEmpty(gym.Description))
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
