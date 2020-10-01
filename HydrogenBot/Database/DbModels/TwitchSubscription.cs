using System;

namespace HydrogenBot.Database.DbModels
{
    public class TwitchSubscription : DbEntity
    {
        public SubscriptionInfo SubscriptionInfo { get; set; } = null!;
        public uint StreamerId { get; set; }
        public bool Online { get; set; }
    }
}
