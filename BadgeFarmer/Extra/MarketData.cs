using System.IO;
using ArchiSteamFarm.Helpers;

namespace BadgeFarmer.Extra
{
    public class MarketData : SerializableFile
    {
        private static string SharedFilePath => Path.Combine(ArchiSteamFarm.SharedInfo.ConfigDirectory, nameof(MarketData) + ".cache");
    }
}