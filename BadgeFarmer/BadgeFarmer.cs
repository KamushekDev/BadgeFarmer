﻿using System;
using System.Collections.Generic;
using System.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using ArchiSteamFarm;
using ArchiSteamFarm.Plugins;

namespace BadgeFarmer
{
    [Export(typeof(IPlugin))]
    public class BadgeFarmer : IBotCommand
    {
        public string Name => "Badge farmer";
        public Version Version => typeof(BadgeFarmer).Assembly.GetName().Version;

        private const string Host = "https://steamcommunity.com/";

        public void OnLoaded()
        {
            Console.WriteLine("a");
        }

        public async Task<string> OnBotCommand(Bot bot, ulong steamID, string message, string[] args)
        {
            var badges = await GetUncompletedBadges(bot);

            foreach (var badge in badges)
            {
                var result = await bot.ArchiWebHandler.UrlGetToHtmlDocumentWithSession(badge.PageUri.ToString(), "/");
                var appIdSegment = badge.PageUri.Segments.Last();
                var r = int.TryParse(appIdSegment.Substring(0, appIdSegment.Length - 1), out var gameId);
                int a = 1;
            }

            return $"There are {badges.Count} uncompleted badges.";
        }

        private async Task<List<Badge>> GetUncompletedBadges(Bot bot)
        {
            byte page = 1;
            var badges = new List<Badge>();
            bool skipRest;
            int badgesCount;

            do
            {
                var badgesPage =
                    await bot.ArchiWebHandler.UrlGetToHtmlDocumentWithSession(ArchiWebHandler.SteamCommunityURL,
                        $"/my/badges?l=english&sort=p&p={page++}");
                badgesCount = ExtractBadgesCount(badgesPage);
                skipRest = ExtractBadgesFromPage(badgesPage, badges);
            } while (badges.Count < badgesCount && !skipRest);

            Debug.Assert(badges.Count <= badgesCount, "Incorrect amount of badges");


            int a = 3;

            return badges.Where(x => !x.IsCompleted).ToList();
        }

        private int ExtractBadgesCount(IDocument badgesPage)
        {
            var badgesCountRegex = new Regex(@"Showing \d+-\d+ of (?<all>\d+) badges",
                RegexOptions.Compiled | RegexOptions.Multiline);
            var badgesCountString = badgesPage.QuerySelector(".profile_paging").TextContent;
            var match = badgesCountRegex.Match(badgesCountString);
            var badgesCount = int.Parse(match.Groups["all"].Value);

            return badgesCount;
        }

        private bool ExtractBadgesFromPage(IDocument badgesPage, List<Badge> allBadges)
        {
            var badgesHtml = badgesPage.QuerySelectorAll(".badge_row");
            var badges = badgesHtml.Select(ExtractBadge).ToArray();


            allBadges.AddRange(badges);

            return badges.Any(x => x.IsCompleted);

            Badge ExtractBadge(IElement element)
            {
                var nameElement = element.QuerySelector(".badge_title");
                nameElement.RemoveChild(nameElement.FirstElementChild);
                var name = nameElement.TextContent.Trim();

                var badgeDescription = element.QuerySelector(".badge_info_description");

                int level = 0;
                if (badgeDescription != null)
                {
                    var levelRegex = new Regex(@"Level (?<level>\d+)", RegexOptions.Multiline | RegexOptions.Compiled);

                    var levelDiv = badgeDescription.Children[1];
                    var match = levelRegex.Match(levelDiv.TextContent);
                    if (match.Success)
                        level = int.Parse(match.Groups["level"].Value);
                }

                var isCompleted = string.IsNullOrWhiteSpace(element.QuerySelector(".badge_progress_info")?.TextContent);

                var linkHtml = element.QuerySelector(".badge_row_overlay");
                var link = ((IHtmlAnchorElement) linkHtml).Href;

                return new Badge(name, isCompleted, level, new Uri(link, UriKind.Absolute));
            }
        }
    }
}