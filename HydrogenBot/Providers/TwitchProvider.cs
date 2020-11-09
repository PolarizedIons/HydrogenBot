using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
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

        private string? GetUsernameFromUrl(string url)
        {
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                return null;
            }

            var uri = new Uri(url);
            if (uri.Host != "twitch.tv" && uri.Host != "www.twitch.tv")
            {
                return null;
            }

            return uri.Segments.Length == 2 ? uri.Segments[1] : null;
        }

        public async Task<ProviderMatchResult> MatchId(string id)
        {
            var username = GetUsernameFromUrl(id);

            if (username == null)
            {
                return new ProviderMatchResult();
            }

            var twitchUserDto = await _twitch.FromName(username);
            return new ProviderMatchResult()
            {
                MatchData = twitchUserDto?.Id ?? 0
            };
        }

        public async Task Subscribe(NotifyString notifyString, object? matchData, ICommandContext commandContext)
        {
            var username = GetUsernameFromUrl(notifyString.ServiceId);
            var twitchId = (uint)(matchData ?? 0);
            if (twitchId == 0)
            {
                await commandContext.Channel.SendMessageAsync($"Error! {username} is not a valid twitch user!");
                return;
            }

            var currentSubscription = await _db.TwitchSubscription
                .Include(x => x.SubscriptionInfo)
                .FirstOrDefaultAsync(x =>
                    x.StreamerId == twitchId &&
                    x.SubscriptionInfo.Channel == notifyString.ChannelId &&
                    x.DeletedAt == null
                );

            if (currentSubscription != null)
            {
                await commandContext.Channel.SendMessageAsync($"Error: <#{commandContext.Channel.Id}> is already subscribed to {username}'s twitch streams!");
                return;
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
            await commandContext.Channel.SendMessageAsync($"<#{commandContext.Channel.Id}> is now subscribed to {username}'s twitch streams!");
        }

        public async Task Unsubscribe(NotifyString notifyString, object? matchData, ICommandContext commandContext)
        {
            var username = GetUsernameFromUrl(notifyString.ServiceId);
            var twitchId = (uint)(matchData ?? 0);
            if (twitchId == 0)
            {
                await commandContext.Channel.SendMessageAsync($"Error! {username} is not a valid twitch user!");
                return;
            }

            var subscription = _db.TwitchSubscription
                .Include(x => x.SubscriptionInfo
                )
                .FirstOrDefault(x => x.StreamerId == twitchId &&
                                     x.SubscriptionInfo.Channel == notifyString.ChannelId &&
                                     x.DeletedAt == null
                );

            if (subscription == null)
            {
                await commandContext.Channel.SendMessageAsync($"Error: <#{commandContext.Channel.Id}> is not subscribed to {username}'s twitch streams!");
                return;
            }

            subscription.DeletedAt = DateTime.UtcNow;
            subscription.SubscriptionInfo.DeletedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            await commandContext.Channel.SendMessageAsync($"<#{commandContext.Channel.Id}> is now unsubscribed from {username}'s twitch streams!");
        }
    }
}
