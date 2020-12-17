using System;
using System.Collections.Generic;
using System.Composition;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using ArchiSteamFarm;
using ArchiSteamFarm.Plugins;
using Newtonsoft.Json;
using SteamKit2;

namespace BadgeFarmer
{
    [Export(typeof(IPlugin))]
    public class BadgeFarmer : IBotCommand, IBotConnection
    {
        public string Name => "Badge farmer";
        public Version Version => typeof(BadgeFarmer)?.Assembly.GetName().Version ?? new Version(0, 0);

        private SteamHelper? SteamHelper;

        public void OnLoaded()
        {
            Console.WriteLine("Badge farmer was loaded");
        }

        public async Task<string> OnBotCommand(Bot bot, ulong steamID, string message, string[] args)
        {
            if (SteamHelper != null)
            {
                var response = await SteamHelper.GetBadges();
                return "Success";
            }

            return "Fault";
        }

        public void OnBotDisconnected(Bot bot, EResult reason)
        {
            SteamHelper = null;
        }

        public void OnBotLoggedOn(Bot bot)
        {
            SteamHelper = new SteamHelper(bot, false);
        }
    }
}