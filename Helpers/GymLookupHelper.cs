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

        public static List<Gym> SearchForGyms(string searchTerm, string groupName)
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
            IEnumerable<Gym> matches = FindNameOrAliasMatch(searchTerm, groupName);
            if (matches.Any()) return matches.ToList();

            // If none found, search for gyms with names which match with Levenshtein distance of <= 2, and territory matches group name
            matches = FindNameOrAliasMatch(searchTerm, groupName, 2);
            if (matches.Any()) return matches.ToList();

            // If none found, search for gyms with names which contain all of the word(s) in the search term, ignoring case or punctuation
            matches = FindNameOrAliasApproximation(searchTerm, groupName);
            if (matches.Any()) return matches.ToList();

            // If none found, search for gyms where the name/alias matches with Levenshtein distance of <= 2, regardless of group territory
            matches = FindNameOrAliasMatch(searchTerm, levenshteinDistance: 2);
            if (matches.Any()) return matches.ToList();

            // If all else has failed, return any approximate matches, regardless of group territory
            matches = FindNameOrAliasApproximation(searchTerm);
            return matches.ToList();
        }

        private static IEnumerable<Gym> FindNameOrAliasMatch(string searchTerm, string groupName = null, int levenshteinDistance = 0)
        {
            var matches = gyms.Where(gym =>
                gym.Name.LevenshteinDistance(searchTerm) <= levenshteinDistance ||
                gym.Aliases.Any(alias => alias.LevenshteinDistance(searchTerm) <= levenshteinDistance)
            );

            if (!string.IsNullOrEmpty(groupName))
            {
                matches = matches.Where(gym => gym.Territory.Contains(groupName));
            }

            return matches;
        }

        private static IEnumerable<Gym> FindNameOrAliasApproximation(string searchTerm, string groupName = null)
        {
            var candidates = gyms.Where(gym =>
            {
                var normalizedSearchTerm = NormalizeStringForGymSearch(searchTerm);
                var normalizedGymName = NormalizeStringForGymSearch(gym.Name);

                var wordsInSearchTerm = normalizedSearchTerm.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                var wordsInGymName = normalizedGymName.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                if (!wordsInSearchTerm.Except(wordsInGymName).Any()) return true;

                return false;
            });

            if (!string.IsNullOrEmpty(groupName))
            {
                candidates = candidates.Where(c => c.Territory.Contains(groupName));
            }

            return candidates;
        }

        private static string NormalizeStringForGymSearch(string input)
        {
            return input
                .ToLowerInvariant()
                .Replace("-", " ")
                .Replace("\\p{P}+", "");
        }

    }
}
