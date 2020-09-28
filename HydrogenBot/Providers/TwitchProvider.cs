using System;
using System.Linq;
using System.Threading.Tasks;
using HydrogenBot.Database;
using HydrogenBot.Database.DbModels;
using HydrogenBot.Models;
using Microsoft.EntityFrameworkCore;

namespace HydrogenBot.Providers
{
    public class TwitchProvider : IProvider
    {
        private readonly DatabaseContext _db;

        public TwitchProvider(DatabaseContext db)
        {
            _db = db;
        }

        public ProviderMatchResult MatchId(string id)
        {
            if (!id.StartsWith("http://") && !id.StartsWith("https://"))
            {
                return new ProviderMatchResult();
            }

            var uri = new Uri(id);
            if (uri.Host != "twitch.tv" && uri.Host != "www.twitch.tv")
            {
                return new ProviderMatchResult();
            }

            if (uri.Segments.Length != 2)
            {
                return new ProviderMatchResult();
            }

            return new ProviderMatchResult()
            {
                MatchData = uri.Segments[1]
            };
        }

        public async Task<bool> Subscribe(NotifyString notifyString, object? matchData)
        {
            var channel = matchData as string;
            if (string.IsNullOrEmpty(channel))
            {
                return false;
            }

            _db.TwitchSubscription.Add(new TwitchSubscription
            {
                Online = false,
                Streamer = channel,
                SubscriptionInfo = new SubscriptionInfo
                {
                    Channel = notifyString.ChannelId,
                    MentionString = notifyString.MentionString
                }
            });

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Unsubscribe(NotifyString notifyString, object? matchData)
        {
            var channel = matchData as string;
            if (string.IsNullOrEmpty(channel))
            {
                return false;
            }

            var subscription = _db.TwitchSubscription
                .Include(x => x.SubscriptionInfo
                )
                .FirstOrDefault(x => x.Streamer == channel &&
                                     x.SubscriptionInfo.Channel == notifyString.ChannelId &&
                                     x.SubscriptionInfo.MentionString == notifyString.MentionString);

            if (subscription == null)
            {
                return false;
            }
            
            _db.Remove(subscription);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
