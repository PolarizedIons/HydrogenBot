namespace HydrogenBot.Models
{
    public class IdentifiedProvider
    {
        public IProvider Provider { get; set; } = null!;
        public object? MatchData { get; set; }
    }
}
