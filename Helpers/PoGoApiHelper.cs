using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PoGoChatbot.Models;

namespace PoGoChatbot.Helpers
{
    public class PoGoApiHelper
    {
        private static readonly HttpClient poGoApiClient = new HttpClient { BaseAddress = new Uri("https://pogoapi.net/") };
        private static List<Pokemon> pokemonList;
        private static Dictionary<string, TypeEffectiveness> typeEffectivenessList;

        private static async Task RefreshPokemonList()
        {
            var response = await poGoApiClient.GetAsync("/api/v1/pokemon_types.json");
            if (response != null && response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                pokemonList = JsonConvert.DeserializeObject<List<Pokemon>>(jsonString);
            }
        }

        private static async Task RefreshTypeEffectivenessList()
        {
            var response = await poGoApiClient.GetAsync("/api/v1/type_effectiveness.json");
            if (response != null && response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                typeEffectivenessList = JsonConvert.DeserializeObject<Dictionary<string, TypeEffectiveness>>(jsonString);
            }
        }

        public static async Task<Pokemon> GetPokemonType(string searchTerm)
        {
            if (pokemonList == null || !pokemonList.Any()) await RefreshPokemonList();
            if (typeEffectivenessList == null || !typeEffectivenessList.Any()) await RefreshTypeEffectivenessList();

            var matchedPokemon = pokemonList
                .FirstOrDefault(pokemon =>
                    (pokemon.Name?.Equals(searchTerm, StringComparison.InvariantCultureIgnoreCase) ?? false) ||
                    (pokemon.Number?.TrimStart('0').Equals(searchTerm.TrimStart('0'), StringComparison.InvariantCultureIgnoreCase) ?? false)
                );

            if (matchedPokemon != null)
            {
                matchedPokemon.TypeEffectiveness = GetPokemonTypeEffectiveness(matchedPokemon.Type);
            }

            return matchedPokemon;
        }

        public static TypeEffectiveness GetPokemonTypeEffectiveness(string[] types)
        {
            return types
                .Select(type =>
                {
                    var properties = typeof(TypeEffectiveness).GetProperties();
                    var typeEffectiveness = new TypeEffectiveness();
                    foreach (var typeMatchup in typeEffectivenessList)
                    {
                        properties
                        .First(p => p.Name == typeMatchup.Key)
                        .SetValue(typeEffectiveness,
                            properties.First(p => p.Name == type)
                            .GetValue(typeMatchup.Value)
                        );
                    }
                    return typeEffectiveness;
                }).Aggregate((a, b) => a * b);
        }
    }
}
