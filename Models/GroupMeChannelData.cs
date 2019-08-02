using System;
using Newtonsoft.Json;

namespace PoGoChatbot.Models
{
    [Serializable]
    public class GroupMeChannelData
    {
        [JsonProperty("attachments")]
        public GroupMeAttachment[]  Attachments { get; set; }
    }

    [Serializable]
    public class GroupMeAttachment
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("lat")]
        public string Latitude { get; set; }
        [JsonProperty("lng")]
        public string Longitude { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
