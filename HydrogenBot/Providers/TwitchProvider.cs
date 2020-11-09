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

        public Task OnSubscribed(ICommandContext context, NotifyString notifyString)
        {
            var username = GetUsernameFromUrl(notifyString.ServiceId);
            return context.Channel.SendMessageAsync($"Success! <#{context.Channel.Id}> is now subscribed to twitch notifications for {username}!");
        }

        public Task OnSubscribedError(ICommandContext context, NotifyString notifyString)
        {
            var username = GetUsernameFromUrl(notifyString.ServiceId);
            return context.Channel.SendMessageAsync($"Error! Could not subscribe to twitch notifications for {username}");
        }

        public Task OnUnsubscribed(ICommandContext context, NotifyString notifyString)
        {
            var username = GetUsernameFromUrl(notifyString.ServiceId);
            return context.Channel.SendMessageAsync($"Success! <#{context.Channel.Id}> is no longer subscribed to twitch notifications for {username}!");
        }

        public Task OnUnsubscribedError(ICommandContext context, NotifyString notifyString)
        {
            var username = GetUsernameFromUrl(notifyString.ServiceId);
            return context.Channel.SendMessageAsync($"Error! Could not unsubscribe to twitch notifications for {username}");
        }
    }
}
