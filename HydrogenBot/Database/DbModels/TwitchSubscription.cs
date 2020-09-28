using System;

namespace HydrogenBot.Database.DbModels
{
    public class TwitchSubscription : DbEntity
    {
        public SubscriptionInfo SubscriptionInfo { get; set; } = null!;
        public string Streamer { get; set; } = null!;
        public bool Online { get; set; }
    }
}
