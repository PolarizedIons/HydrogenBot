namespace HydrogenBot.Database.DbModels
{
    public class SubscriptionInfo : DbEntity
    {
        public ulong Channel { get; set; }
        public string? MentionString { get; set; }
    }
}
