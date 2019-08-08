using System;
using Newtonsoft.Json;

namespace PoGoChatbot.Models
{
    [Serializable]
    public class GroupMeChannelData
    {
        public GroupMeAttachment Attachment { get; set; }
    }

    [Serializable]
    public class GroupMeAttachment
    {
        [JsonProperty("type")]
        virtual public string Type { get; set; }
    }

    [Serializable]
    public class GroupMeLocationAttachment : GroupMeAttachment
    {
        [JsonProperty("type")]
        public override string Type => "location";
        [JsonProperty("lat")]
        public string Latitude { get; set; }
        [JsonProperty("lng")]
        public string Longitude { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    [Serializable]
    public class GroupMeImageAttachment : GroupMeAttachment
    {
        [JsonProperty("type")]
        public override string Type => "image";
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
