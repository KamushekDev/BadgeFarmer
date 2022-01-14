using System;
using System.Composition;
using System.Threading.Tasks;
using ArchiSteamFarm;
using ArchiSteamFarm.Plugins;
using BadgeFarmer.Clients;
using BadgeFarmer.Commands;
using BadgeFarmer.Services;
using Microsoft.Extensions.DependencyInjection;
using SteamKit2;

namespace BadgeFarmer
{
    [Export(typeof(IPlugin))]
    public class BadgeFarmer : IBotCommand, IBotConnection
    {
        public string Name => "Badge farmer";
        public Version Version => typeof(BadgeFarmer)?.Assembly.GetName().Version ?? new Version(0, 1);

        private IServiceProvider _serviceProvider;

        private bool _isActive = false;

        public void OnLoaded()
        {
            Console.WriteLine($"{nameof(BadgeFarmer)} plugin was loaded.");
        }

        public async Task<string> OnBotCommand(Bot bot, ulong steamId, string message, string[] args)
        {
            if (!_isActive)
                return "Badge farmer isn't active. Does the bot correctly logged in?";

            var command = new CommandParser().Parse(message);
            try
            {
                var executor = _serviceProvider.GetRequiredService<ICommandExecutor>();
                var result = await executor.Execute(command);
                return result;
            }
            catch (Exception e)
            {
                bot.ArchiLogger.LogGenericException(e);
                throw;
            }
        }

        public void OnBotDisconnected(Bot bot, EResult reason)
        {
            _isActive = false;
        }

        public void OnBotLoggedOn(Bot bot)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IFileSaver, FileSaver>();
            serviceCollection.AddSingleton<ICustomSteamClient>(new CustomSteamClient(bot));
            serviceCollection.AddSingleton<ICardsService, CardsService>();
            serviceCollection.AddSingleton<IBadgesService, BadgesService>();
            serviceCollection.AddSingleton<IInventoryService, InventoryService>();
            
            serviceCollection.AddSingleton<ICommandExecutor, CommandExecutor>();

            _serviceProvider = serviceCollection.BuildServiceProvider(true);

            _isActive = true;
        }
    }
}