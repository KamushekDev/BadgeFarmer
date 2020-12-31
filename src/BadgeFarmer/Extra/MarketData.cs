using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ArchiSteamFarm;
using ArchiSteamFarm.Helpers;
using BadgeFarmer.Models;
using BadgeFarmer.Models.Responses;
using Newtonsoft.Json;

namespace BadgeFarmer.Extra
{
    public class MarketData : SerializableFile
    {
        private static string SharedFilePath =>
            Path.Combine(SharedInfo.ConfigDirectory, nameof(MarketData) + ".cache");

        [JsonProperty("cards")] public List<SearchEntry> Cards { get; set; }
        [JsonProperty("gameIds")] public SortedSet<long> GameIds { get; set; }
        [JsonProperty("skippedGameIds")] public SortedSet<long> SkippedGameIds { get; set; }
        [JsonProperty("badgeCards")] public List<BadgeCards> BadgeCardsList { get; set; }

        private MarketData()
        {
            Cards = new List<SearchEntry>();
            GameIds = new SortedSet<long>();
            SkippedGameIds = new SortedSet<long>();
            BadgeCardsList = new List<BadgeCards>();
            FilePath = SharedFilePath;
        }

        internal static async Task<MarketData> Load()
        {
            if (!File.Exists(SharedFilePath))
            {
                return new MarketData();
            }

            MarketData? marketData = null;

            try
            {
                string json = await RuntimeCompatibility.File.ReadAllTextAsync(SharedFilePath).ConfigureAwait(false);

                if (!string.IsNullOrEmpty(json))
                {
                    marketData = JsonConvert.DeserializeObject<MarketData>(json);
                }
            }
            catch (Exception e)
            {
                ASF.ArchiLogger.LogGenericException(e);
            }

            if (marketData == null)
            {
                ASF.ArchiLogger.LogGenericError(
                    $"{nameof(MarketData)} could not be loaded, a fresh instance will be initialized.");

                marketData = new MarketData();
            }

            return marketData;
        }

        public new Task Save()
        {
            return base.Save();
        }
    }
}