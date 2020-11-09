using System;
using System.Linq;
using System.Threading.Tasks;
using HydrogenBot.Database;
using HydrogenBot.Database.DbModels;
using HydrogenBot.Models;
using HydrogenBot.Services;
using Microsoft.EntityFrameworkCore;

namespace HydrogenBot.Providers
{
    public class TwitchProvider : IProvider
    {
        private readonly DatabaseContext _db;
        private readonly TwitchService _twitch;

        public TwitchProvider(DatabaseContext db, TwitchService twitch)
        {
            _db = db;
            _twitch = twitch;
        }

        public async Task<ProviderMatchResult> MatchId(string id)
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

            var twitchUserDto = await _twitch.FromName(uri.Segments[1]);
            return new ProviderMatchResult()
            {
                MatchData = twitchUserDto?.Id ?? 0
            };
        }

        public async Task<bool> Subscribe(NotifyString notifyString, object? matchData)
        {
            var twitchId = (uint)(matchData ?? 0);
            if (twitchId == 0)
            {
                return false;
            }

            var streamInfo = (await _twitch.BatchStreamInfo(new[] {twitchId})).ToArray();
            _db.TwitchSubscription.Add(new TwitchSubscription
            {
                Online = streamInfo.Length > 0,
                StreamerId = twitchId,
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
            var twitchId = (uint)(matchData ?? 0);
            if (twitchId == 0)
            {
                return false;
            }

            var subscription = _db.TwitchSubscription
                .Include(x => x.SubscriptionInfo
                )
                .FirstOrDefault(x => x.StreamerId == twitchId &&
                                     x.SubscriptionInfo.Channel == notifyString.ChannelId &&
                                     x.SubscriptionInfo.MentionString == notifyString.MentionString &&
                                     x.DeletedAt == null);

            if (subscription == null)
            {
                return false;
            }

            subscription.DeletedAt = DateTime.UtcNow;
            subscription.SubscriptionInfo.DeletedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return true;
        }

        public string SubscribedText => "I'll notify you when they go live.";
        public string UnsubscribedText => "I'll stop notifying you when they go live.";
    }
}
