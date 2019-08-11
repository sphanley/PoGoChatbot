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
        public string[] Type { get; set; }

        public TypeEffectiveness TypeEffectiveness { get; set; }
    }

    [Serializable]
    public class TypeEffectiveness
    {
        public double Bug { get; set; }
        public double Dark { get; set; }
        public double Dragon { get; set; }
        public double Electric { get; set; }
        public double Fairy { get; set; }
        public double Fighting { get; set; }
        public double Fire { get; set; }
        public double Flying { get; set; }
        public double Ghost { get; set; }
        public double Grass { get; set; }
        public double Ground { get; set; }
        public double Ice { get; set; }
        public double Normal { get; set; }
        public double Poison { get; set; }
        public double Psychic { get; set; }
        public double Rock { get; set; }
        public double Steel { get; set; }
        public double Water { get; set; }

        public static TypeEffectiveness operator * (TypeEffectiveness a, TypeEffectiveness b)
        {
            return new TypeEffectiveness {
                Bug = a.Bug * b.Bug,
                Dark = a.Dark * b.Dark,
                Dragon = a.Dragon * b.Dragon,
                Electric = a.Electric * b.Electric,
                Fairy = a.Fairy * b.Fairy,
                Fighting = a.Fighting * b.Fighting,
                Fire = a.Fire * b.Fire,
                Flying = a.Flying * b.Flying,
                Ghost = a.Ghost * b.Ghost,
                Grass = a.Grass * b.Grass,
                Ground = a.Ground * b.Ground,
                Ice = a.Ice * b.Ice,
                Normal = a.Normal * b.Normal,
                Poison = a.Poison * b.Poison,
                Psychic = a.Psychic * b.Psychic,
                Rock = a.Rock * b.Rock,
                Steel = a.Steel * b.Steel,
                Water = a.Water * b.Water
            };
        }
    }

}
