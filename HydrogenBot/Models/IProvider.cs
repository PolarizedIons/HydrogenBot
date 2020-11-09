using System.Threading.Tasks;
using Discord.Commands;

namespace HydrogenBot.Models
{
    public interface IProvider : ISingletonDiService
    {
        Task<ProviderMatchResult> MatchId(string id);
        Task<bool> Subscribe(NotifyString notifyString, object? matchData);
        Task<bool> Unsubscribe(NotifyString notifyString, object? matchData);

        public Task OnSubscribed(ICommandContext context, NotifyString notifyString);
        Task OnSubscribedError(ICommandContext context, NotifyString notifyString);
        public Task OnUnsubscribed(ICommandContext context, NotifyString notifyString);
        Task OnUnsubscribedError(ICommandContext context, NotifyString notifyString);
    }
}
