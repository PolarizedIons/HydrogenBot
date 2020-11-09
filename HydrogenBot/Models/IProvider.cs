using System.Threading.Tasks;
using Discord.Commands;

namespace HydrogenBot.Models
{
    public interface IProvider : ISingletonDiService
    {
        Task<ProviderMatchResult> MatchId(string id);
        Task Subscribe(NotifyString notifyString, object? matchData, ICommandContext commandContext);
        Task Unsubscribe(NotifyString notifyString, object? matchData, ICommandContext commandContext);
    }
}
