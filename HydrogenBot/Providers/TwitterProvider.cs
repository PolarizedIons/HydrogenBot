using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using HydrogenBot.Database;
using HydrogenBot.Database.DbModels;
using HydrogenBot.Models;
using Microsoft.EntityFrameworkCore;

namespace HydrogenBot.Providers
{
    public class TwitterProvider : IProvider
    {
        private readonly DatabaseContext _db;

        public TwitterProvider(DatabaseContext db)
        {
            _db = db;
        }

        private string? GetUsernameFromUrl(string url)
        {
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                return null;
            }

            var uri = new Uri(url);
            if (uri.Host != "twitter.com" && uri.Host != "www.twitter.com")
            {
                return null;
            }

            return uri.Segments.Length == 2 ? uri.Segments[1] : null;
        }

        public Task<ProviderMatchResult> MatchId(string id)
        {
            var username = GetUsernameFromUrl(id);

            if (username == null)
            {
                return Task.FromResult(new ProviderMatchResult());
            }

            return Task.FromResult(new ProviderMatchResult()
            {
                MatchData = username
            });
        }

        public async Task Subscribe(NotifyString notifyString, object? matchData, ICommandContext commandContext)
        {
            if (notifyString.ServiceConfig.ToLowerInvariant() != "tweets")
            {
                await commandContext.Channel.SendMessageAsync($"Error! Invalid action; Valid actions: `tweets`!");
                return;
            }

            var username = (string)(matchData ?? "");
            if (string.IsNullOrEmpty(username))
            {
                await commandContext.Channel.SendMessageAsync($"Error! That is not a valid twitter user!");
                return;
            }

            var currentSubscription = await _db.TwitterSubscription
                .Include(x => x.SubscriptionInfo)
                .FirstOrDefaultAsync(x =>
                    x.Username == username &&
                    x.SubscriptionInfo.Channel == notifyString.ChannelId &&
                    x.DeletedAt == null
                );

            if (currentSubscription != null)
            {
                await commandContext.Channel.SendMessageAsync($"Error: <#{notifyString.ChannelId}> is already subscribed to {username}'s tweets!");
                return;
            }

            _db.TwitterSubscription.Add(new TwitterSubscription()
            {
                Username = username,
                SubscriptionInfo = new SubscriptionInfo
                {
                    Channel = notifyString.ChannelId,
                    MentionString = notifyString.MentionString
                }
            });

            await _db.SaveChangesAsync();
            await commandContext.Channel.SendMessageAsync($"<#{notifyString.ChannelId}> is now subscribed to {username}'s tweets!");
        }

        public async Task Unsubscribe(NotifyString notifyString, object? matchData, ICommandContext commandContext)
        {
            if (notifyString.ServiceConfig.ToLowerInvariant() != "tweets")
            {
                await commandContext.Channel.SendMessageAsync($"Error! Invalid action; Valid actions: `tweets`!");
                return;
            }

            var username = (string)(matchData ?? "");
            if (string.IsNullOrEmpty(username))
            {
                await commandContext.Channel.SendMessageAsync($"Error! That is not a valid twitter user!");
                return;
            }

            var subscription = _db.TwitterSubscription
                .Include(x => x.SubscriptionInfo
                )
                .FirstOrDefault(x => x.Username == username &&
                                     x.SubscriptionInfo.Channel == notifyString.ChannelId &&
                                     x.DeletedAt == null
                );

            if (subscription == null)
            {
                await commandContext.Channel.SendMessageAsync($"Error: <#{notifyString.ChannelId}> is not subscribed to {username}'s tweets!");
                return;
            }

            subscription.DeletedAt = DateTime.UtcNow;
            subscription.SubscriptionInfo.DeletedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            await commandContext.Channel.SendMessageAsync($"<#{notifyString.ChannelId}> is now unsubscribed from {username}'s tweets!");
        }
    }
}
