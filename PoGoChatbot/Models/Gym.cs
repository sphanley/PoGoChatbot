using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PoGoChatbot.Models
{
    [Serializable]
    public class Gym
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("aliases")]
        public List<string> Aliases { get; set; } = new List<string>();
        [JsonProperty("territory")]
        public List<string> Territory { get; set; } = new List<string>();
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("is_ex_eligible")]
        public bool IsEXEligible { get; set; }
        [JsonProperty("location")]
        public LatLng Location { get; set; }
    }
}
