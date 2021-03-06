﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PoGoChatbot.Models;

namespace PoGoChatbot.Services
{
    public static class PoGoApi
    {
        private static readonly HttpClient poGoApiClient = new HttpClient { BaseAddress = new Uri("https://pogoapi.net/") };
        private static List<Pokemon> pokemonList = new List<Pokemon>();
        private static TypeMatchupList typeMatchupList = new TypeMatchupList();

        public static async Task<Pokemon> GetPokemon(string searchTerm)
        {
            if (!pokemonList.Any()) pokemonList = await LoadDataFromApi<List<Pokemon>>("/api/v1/pokemon_types.json");
            if (!typeMatchupList.Any()) typeMatchupList = await LoadDataFromApi<TypeMatchupList>("/api/v1/type_effectiveness.json");

            var matchedPokemon = pokemonList.FirstOrDefault(pokemon =>
                (pokemon.Name?.Equals(searchTerm, StringComparison.InvariantCultureIgnoreCase) ?? false) ||
                (pokemon.Number?.TrimStart('0').Equals(searchTerm.TrimStart('0'), StringComparison.InvariantCultureIgnoreCase) ?? false)
            );

            if (matchedPokemon != null)
            {
                matchedPokemon.MatchupsForType = GetMatchupsForType(matchedPokemon.Type);
            }

            return matchedPokemon;
        }

        private static async Task<T> LoadDataFromApi<T>(string route)
        {
            var response = await poGoApiClient.GetAsync(route);
            if (response != null && response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(jsonString);
            }
            return default(T);
        }

        private static MatchupsForType GetMatchupsForType(List<string> pokemonTypes)
        {
            return pokemonTypes
                .Select(pokemonType =>
                {
                    var matchupsForType = new MatchupsForType();

                    foreach (var matchup in typeMatchupList)
                    {
                        matchupsForType[matchup.Key] = matchup.Value.First(row => row.Key == pokemonType).Value;
                    }

                    return matchupsForType;
                })
                .Aggregate((a, b) => a * b);
        }
    }
}
