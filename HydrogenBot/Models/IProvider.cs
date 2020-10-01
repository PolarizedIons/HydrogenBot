using System.Threading.Tasks;

namespace HydrogenBot.Models
{
    public interface IProvider : ISingletonDiService
    {
        Task<ProviderMatchResult> MatchId(string id);
        Task<bool> Subscribe(NotifyString notifyString, object? matchData);
        Task<bool> Unsubscribe(NotifyString notifyString, object? matchData);

        public string SubscribedText { get; }
        public string UnsubscribedText { get; }
    }
}
