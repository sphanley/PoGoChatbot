using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PoGoChatbot.Models
{
    [Serializable]
    public class Pokemon
    {
        [JsonProperty("pokemon_name")]
        public string Name { get; set; }
        [JsonProperty("pokemon_id")]
        public string Number { get; set; }
        [JsonProperty("type")]
        public List<string> Type { get; set; } = new List<string>();

        public MatchupsForType MatchupsForType { get; set; }
    }
}
