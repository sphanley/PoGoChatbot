using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DuoVia.FuzzyStrings;
using Newtonsoft.Json;
using PoGoChatbot.Models;

namespace PoGoChatbot
{
    public static class GymLookupHelper
    {
        public static List<Gym> SearchForGyms(String searchTerm)
        {

            using (StreamReader r = new StreamReader("Resources/gym_locations.json"))
            {
                string json = r.ReadToEnd();
                var gymList = JsonConvert.DeserializeObject<List<Gym>>(json);

                // Search for gyms with names which match exactly
                var exactMatch = gymList.Where(gym => gym.Name.ToLowerInvariant().Equals(searchTerm.ToLowerInvariant()));
                if (exactMatch.Count() != 0) return exactMatch.ToList();

                // If none found, search for gyms with names which match approximately
                var fuzzyMatches = gymList.Where(gym => gym.Name.FuzzyEquals(searchTerm));
                if (fuzzyMatches.Count() != 0) return fuzzyMatches.ToList();


                // If none found, search for gyms with names which contain all of the word(s) in the search term, ignoring case or punctuation
                var gymMatches = gymList.Where(gym =>
                {
                    var normalizedSearchTerm = searchTerm.Replace("\\p{P}+", "");
                    var normalizedGymName = gym.Name.ToLowerInvariant().Replace("\\p{P}+", "");

                    var wordsInSearchTerm = normalizedSearchTerm.Split(" ");
                    var wordsInGymName = normalizedGymName.Split(" ");

                    if (!wordsInSearchTerm.Except(wordsInGymName).Any()) return true;

                    return false;
                });

                return gymMatches.ToList();
            }
        }
    }
}
