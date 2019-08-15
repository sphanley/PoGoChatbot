﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DuoVia.FuzzyStrings;
using Newtonsoft.Json;
using PoGoChatbot.Models;

namespace PoGoChatbot.Helpers
{
    public static class GymLookupHelper
    {
        private static List<Gym> gyms;

        public static List<Gym> SearchForGyms(string searchTerm)
        {
            if (gyms == null || !gyms.Any())
            {
                using (StreamReader r = new StreamReader("Resources/gym_locations.json"))
                {
                    string json = r.ReadToEnd();
                    gyms = JsonConvert.DeserializeObject<List<Gym>>(json);
                }
            }

            // Search for gyms with names which match exactly
            var exactMatch = gyms.Where(gym => gym.Name.ToLowerInvariant().Equals(searchTerm.ToLowerInvariant()));
            if (exactMatch.Any()) return exactMatch.ToList();

            // Search for gyms with an alias which matches exactly
            var aliasMatch = gyms.Where(gym => gym.Aliases.Any(a => a.ToLowerInvariant().Equals(searchTerm.ToLowerInvariant())));
            if (aliasMatch.Any()) return aliasMatch.ToList();

            // If none found, search for gyms with names which match approximately
            var fuzzyMatches = gyms.Where(gym => gym.Name.FuzzyEquals(searchTerm));
            if (fuzzyMatches.Any()) return fuzzyMatches.ToList();

            // If none found, search for gyms with names which contain all of the word(s) in the search term, ignoring case or punctuation
            var gymMatches = gyms.Where(gym =>
            {
                var normalizedSearchTerm = Normalize(searchTerm);
                var normalizedGymName = Normalize(gym.Name);

                var wordsInSearchTerm = normalizedSearchTerm.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                var wordsInGymName = normalizedGymName.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                if (!wordsInSearchTerm.Except(wordsInGymName).Any()) return true;

                return false;
            });

            return gymMatches.ToList();

        }

        private static string Normalize(string input)
        {
            return input
                .ToLowerInvariant()
                .Replace("-", " ")
                .Replace("\\p{P}+", "");
        }
    }
}
