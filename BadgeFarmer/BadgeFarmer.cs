using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BadgeFarmer
{
    [Export(typeof(IPlugin))]
    public class BadgeFarmer : IBotSteamClient, IBotCommand {
        public string Name => "Badge farmer";
        public Version Version => new Version(0, 1);

        public void OnLoaded() {
            Console.WriteLine("a");
        }

        public Task<string> OnBotCommand(Bot bot, ulong steamID, string message, string[] args) => throw new NotImplementedException();

        public void OnBotSteamCallbacksInit(Bot bot, CallbackManager callbackManager) { }

        public IReadOnlyCollection<ClientMsgHandler> OnBotSteamHandlersInit(Bot bot) {
            return new ClientMsgHandler[0];
        }
    }
}