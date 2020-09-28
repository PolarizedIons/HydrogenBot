using System.Threading.Tasks;

namespace HydrogenBot.Models
{
    public interface IProvider : ISingletonDiService
    {
        ProviderMatchResult MatchId(string id);
        Task<bool> Subscribe(NotifyString notifyString, object? matchData);
        Task<bool> Unsubscribe(NotifyString notifyString, object? matchData);
    }
}
