using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PoGoChatbot.Models;

namespace PoGoChatbot.Services
{
    public static class PokeBattlerApi
    {
        private static readonly HttpClient pokeBattlerApi = new HttpClient { BaseAddress = new Uri("https://fight.pokebattler.com/") };
        private static RaidList raidList = new RaidList();
        private static DateTime raidListExpirationDateTime = DateTime.MinValue;

        public static async Task Initialize()
        {
            raidList = await LoadDataFromApi<RaidList>("raids");
        }

        private static async Task<T> LoadDataFromApi<T>(string route)
        {
            using (var response = await pokeBattlerApi.GetAsync(route))
            {
                if (response != null && response.IsSuccessStatusCode)
                {
                    raidListExpirationDateTime = DateTime.Now.Add(response.Headers.CacheControl.MaxAge ?? TimeSpan.Zero);

                    var jsonString = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(jsonString);
                }
            }

            return default(T);
        }

        public static async Task<List<Raid>> GetRaids()
        {
            if (!raidList.Tiers.Any() || raidListExpirationDateTime < DateTime.Now) raidList = await LoadDataFromApi<RaidList>("raids");

            return raidList.Tiers?.Select(t => t.Raids)?.SelectMany(raids => raids)?.ToList() ?? new List<Raid>();

        }

        public static async Task<List<Raid>> GetRaids(int tier)
        {
            if (!raidList.Tiers.Any() || raidListExpirationDateTime < DateTime.Now) raidList = await LoadDataFromApi<RaidList>("raids");

            var raidTier = raidList.Tiers.FirstOrDefault(t => t.Name == $"RAID_LEVEL_{tier}");

            return raidTier?.Raids ?? new List<Raid>();
        }
    }
}
