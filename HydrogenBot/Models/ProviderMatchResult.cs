namespace HydrogenBot.Models
{
    public struct ProviderMatchResult
    {
        public bool IsSuccess => MatchData != null;
        public object? MatchData { get; set; }
    }
}
