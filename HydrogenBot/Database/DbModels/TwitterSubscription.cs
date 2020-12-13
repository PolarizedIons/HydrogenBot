namespace HydrogenBot.Database.DbModels
{
    public class TwitterSubscription : DbEntity
    {
        public SubscriptionInfo SubscriptionInfo { get; set; } = null!;
        public string Username { get; set; } = null!;
    }
}
