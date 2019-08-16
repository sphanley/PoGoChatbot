using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Bot.Builder;
using Newtonsoft.Json;

namespace PoGoChatbot
{
    public static class VariableResources
    {
        private static Dictionary<string, string> gymMapUrlMappings;

        public static string GetMapUrl(ITurnContext turnContext)
        {
            if (gymMapUrlMappings == null || !gymMapUrlMappings.Any())
            {
                gymMapUrlMappings = JsonConvert.DeserializeObject<Dictionary<string, string>>(Environment.GetEnvironmentVariable("MapUrlMappings"));
            }
            return gymMapUrlMappings.GetValueOrDefault(turnContext.Activity.Conversation.Id, "");
        }
    }
}
