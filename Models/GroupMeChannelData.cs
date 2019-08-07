using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PoGoChatbot.Models
{
    [Serializable]
    public class GroupMeChannelData : JArray
    {
        public void Add(GroupMeAttachment attachment)
        {
            Add(JToken.FromObject(attachment));
        }
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
