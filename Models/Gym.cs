using Newtonsoft.Json;

namespace PoGoChatbot.Models
{
    public class Gym
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("is_ex_eligible")]
        public bool IsEXEligible { get; set; }
        [JsonProperty("location")]
        public LatLng Location { get; set; }
        [JsonProperty("aliases")]
        public string[] Aliases { get; set; } = new string[0];
    }
}
