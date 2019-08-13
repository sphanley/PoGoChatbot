using System;
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
        public string[] Type { get; set; } = new string[0];

        public MatchupsForType MatchupsForType { get; set; }
    }
}
