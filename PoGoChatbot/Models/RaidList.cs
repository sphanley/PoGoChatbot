using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PoGoChatbot.Models
{
    [Serializable]
    public class RaidList
    {
        [JsonProperty("tiers")]
        public List<RaidTier> Tiers { get; set; } = new List<RaidTier>();
    }

    [Serializable]
    public class TierInfo
    {
        [JsonProperty("hp")]
        public int HP { get; set; }
        [JsonProperty("cpm")]
        public double CPM { get; set; }
        [JsonProperty("level")]
        public string Level { get; set; }
        [JsonProperty("eggURL")]
        public string EggURL { get; set; }
        [JsonProperty("guessTier")]
        public string GuessTier { get; set; }
        [JsonProperty("attackMultiplier")]
        public double AttackMultiplier { get; set; }
        [JsonProperty("defenseMultiplier")]
        public double DefenseMultiplier { get; set; }
        [JsonProperty("staminaMultiplier")]
        public double StaminaMultiplier { get; set; }
    }

    [Serializable]
    public class RaidTier
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("tier")]
        public string Name { get; set; }
        [JsonProperty("info")]
        public TierInfo Info { get; set; }
        [JsonProperty("raids")]
        public List<Raid> Raids { get; set; } = new List<Raid>();
        [JsonProperty("attackMultiplier")]
        public double AttackMultiplier { get; set; }
        [JsonProperty("defenseMultiplier")]
        public double DefenseMultiplier { get; set; }
        [JsonProperty("staminaMultiplier")]
        public double StaminaMultiplier { get; set; }
        [JsonProperty("raidType")]
        public string RaidType { get; set; }
    }

    [Serializable]
    public class Article
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("articleSlug")]
        public string ArticleSlug { get; set; }
        [JsonProperty("articleThumbnailURL")]
        public string ArticleThumbnailURL { get; set; }
        [JsonProperty("articleURL")]
        public string ArticleURL { get; set; }
        [JsonProperty("infographicURL")]
        public string InfographicURL { get; set; }
        [JsonProperty("infographicThumbnailURL")]
        public string InfographicThumbnailURL { get; set; }
        [JsonProperty("infographicShareURL")]
        public string InfographicShareURL { get; set; }
        [JsonProperty("infographicShareWidth")]
        public int InfographicShareWidth { get; set; }
        [JsonProperty("infographicShareHeight")]
        public int InfographicShareHeight { get; set; }
        [JsonProperty("newsCategorySlug")]
        public string NewsCategorySlug { get; set; }
        [JsonProperty("newsTagSlug")]
        public string NewsTagSlug { get; set; }
        [JsonProperty("videoId")]
        public string VideoId { get; set; }
    }

    [Serializable]
    public class Raid
    {
        [JsonProperty("id")]
        public object Id { get; set; }
        [JsonProperty("pokemon")]
        public string Pokemon { get; set; }
        [JsonProperty("cp")]
        public int CP { get; set; }
        [JsonProperty("shiny")]
        public bool Shiny { get; set; }
        [JsonProperty("verified")]
        public bool Verified { get; set; }
        [JsonProperty("minCp")]
        public int MinCP { get; set; }
        [JsonProperty("article")]
        public Article Article { get; set; }
    }

}
