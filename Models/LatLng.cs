using System;
using Newtonsoft.Json;

namespace PoGoChatbot.Models
{
    [Serializable]
    public class LatLng
    {
        [JsonProperty("latitude")]
        public string Latitude { get; set; }
        [JsonProperty("longitude")]
        public string Longitude { get; set; }
    }
}
