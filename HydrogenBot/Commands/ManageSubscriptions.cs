using System.Text.Json;
using System.Threading.Tasks;
using Discord.Commands;
using HydrogenBot.Models;
using HydrogenBot.Services;

namespace HydrogenBot.Commands
{
    public class ManageSubscriptions : ModuleBase
    {
        public const string Usage = "`notify (channel) [mentioning (role)] when (service) (action)`";
        private readonly ProviderManagerService _providerManagerService;

        public ManageSubscriptions(ProviderManagerService providerManagerService)
        {
            _providerManagerService = providerManagerService;
        }

        [Command("notify")]
        public async Task Subscribe([Remainder] NotifyString notifyString)
        {
            var providerId = await _providerManagerService.Identify(notifyString.ServiceId);
            if (providerId == null)
            {
                await ReplyAsync("No such service; Usage: " + Usage);
                return;
            }

            var success = await providerId.Provider.Subscribe(notifyString, providerId.MatchData);
            if (success)
            {
                await ReplyAsync("Success: " + providerId.Provider.SubscribedText);
            }
            else
            {
                await ReplyAsync("Error: Could not subscribe you");
            }
        }

        [Command("stop notifying")]
        public async Task Unsubscribe([Remainder] NotifyString notifyString)
        {
            var providerId = await _providerManagerService.Identify(notifyString.ServiceId);
            if (providerId == null)
            {
                await ReplyAsync("No such service; Usage: " + Usage);
                return;
            }

            var success = await providerId.Provider.Unsubscribe(notifyString, providerId.MatchData);
            if (success)
            {
                await ReplyAsync("Success: " + providerId.Provider.UnsubscribedText);
            }
            else
            {
                await ReplyAsync("Error: Could not unsubscribe you.");
            }
        }
    }
}
