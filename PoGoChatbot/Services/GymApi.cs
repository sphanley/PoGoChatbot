using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DuoVia.FuzzyStrings;
using Newtonsoft.Json;
using PoGoChatbot.Models;

namespace PoGoChatbot.Services
{
    public static class GymApi
    {
        private static List<Gym> gyms;

        public static List<Gym> GetGyms(string searchTerm, string groupName, int? maximum = null)
        {
            if (gyms == null || !gyms.Any())
            {
                using (StreamReader r = new StreamReader("Resources/gym_locations.json"))
                {
                    string json = r.ReadToEnd();
                    gyms = JsonConvert.DeserializeObject<List<Gym>>(json);
                }
            }

            // Search for gyms with names or aliases which match exactly 
            IEnumerable<Gym> matches = FindNameMatch(searchTerm, groupName);
            if (matches.Any()) return matches.ToList();

            // If none found, search for gyms with names which match with Levenshtein distance of <= 2, and territory matches group name
            matches = FindNameMatch(searchTerm, groupName, 2, false);
            if (matches.Any()) return matches.ToList();

            // If none found, search for gyms with names which contain all of the word(s) in the search term, ignoring case or punctuation
            matches = FindNameOrAliasApproximation(searchTerm, groupName);
            if (matches.Any()) return matches.ToList();

            // If none found, search for gyms where the name/alias matches with Levenshtein distance of <= 2, regardless of group territory
            matches = FindNameMatch(searchTerm, maxLevenshteinDistance: 2);
            if (matches.Any()) return matches.ToList();

            // If all else has failed, return any approximate matches, regardless of group territory
            matches = FindNameOrAliasApproximation(searchTerm);

            if (maximum != null) matches = matches.Take(maximum.Value);
            return matches.ToList();
        }

        private static IEnumerable<Gym> FindNameMatch(string searchTerm, string groupName = null, int maxLevenshteinDistance = 0, bool shouldSearchAliases = true)
        {

            var matches = gyms.Where(gym =>
            {
                // Enforce maximum Levenshtien distance of one less than half of gym name length, to avoid overly fuzzing gyms with very short names
                var levenshtienDistance = Math.Min(maxLevenshteinDistance, (gym.Name.Length / 2)-1);

                if (gym.Name.LevenshteinDistance(searchTerm) <= levenshtienDistance) return true;
                if (shouldSearchAliases && gym.Aliases.Any(alias => alias.Equals(searchTerm, StringComparison.InvariantCultureIgnoreCase))) return true;
                return false;
            });

            if (!string.IsNullOrEmpty(groupName)) matches = matches.Where(gym => gym.Territory.Contains(groupName));

            return matches;
        }

        private static IEnumerable<Gym> FindNameOrAliasApproximation(string searchTerm, string groupName = null)
        {
            var wordsInSearchTerm = SplitIntoWords(searchTerm.ToLowerInvariant());

            var candidates = gyms.Where(gym =>
                SplitIntoWords(gym.Name.ToLowerInvariant()).IsPosessivenessAgnosticSupersetOf(wordsInSearchTerm) ||
                gym.Aliases.Any(alias => SplitIntoWords(alias.ToLowerInvariant()).IsPosessivenessAgnosticSupersetOf(wordsInSearchTerm))
            );

            if (!string.IsNullOrEmpty(groupName)) candidates = candidates.Where(c => c.Territory.Contains(groupName));

            return candidates;
        }

        private static IList<string> SplitIntoWords(string input)
        {
            return input.Split(" ", StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Split("-"))
                        .SelectMany(s => s)
                        .ToList();
        }

        private static bool IsPosessivenessAgnosticSupersetOf(this IList<string> superset, IList<string> subset)
        {
            var permutations = superset.ToList();
            // For each word, create a posessive form permutation with a trailing "s",
            // as well as a version with the trailing "s" removed if present initially
            foreach (var str in superset)
            {
                if (str.EndsWith('s')) permutations.Add(str.Substring(0, str.Length - 1));
                permutations.Add($"{str}s");
            }

            // Check whether any words exist in the subset which are not in the superset or acceptable permutations
            return !subset.Except(permutations).Any();
        }
    }
}
