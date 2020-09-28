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
            await ReplyAsync(JsonSerializer.Serialize(notifyString));

            var provider = _providerManagerService.Identify(notifyString.ServiceId, out var matchData);
            if (provider == null)
            {
                await ReplyAsync("No such service; Usage: " + Usage);
                return;
            }

            await provider.Subscribe(notifyString, matchData);
        }

        [Command("stop notifying")]
        public async Task Unsubscribe([Remainder] NotifyString notifyString)
        {
            await ReplyAsync(JsonSerializer.Serialize(notifyString));

            var provider = _providerManagerService.Identify(notifyString.ServiceId, out var matchData);
            if (provider == null)
            {
                await ReplyAsync("No such service; Usage: " + Usage);
                return;
            }

            await provider.Unsubscribe(notifyString, matchData);
        }
    }
}
