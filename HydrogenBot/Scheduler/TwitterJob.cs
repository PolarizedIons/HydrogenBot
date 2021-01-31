using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using HydrogenBot.Database;
using HydrogenBot.Database.DbModels;
using HydrogenBot.Services;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Serilog;

namespace HydrogenBot.Scheduler
{
    public class TwitterJob : BaseJob
    {
        private const string TweetUrlTemplate = "https://twitter.com/{username}/status/{id}";

        public new static int SecondsInterval => 60;
        private static readonly List<string> SeenUsernames = new List<string>();

        private readonly TwitterService _twitter;
        private readonly DatabaseContext _db;
        private readonly DiscordSocketClient _discord;

        public TwitterJob(TwitterService twitter, DatabaseContext db, DiscordSocketClient discord)
        {
            _twitter = twitter;
            _db = db;
            _discord = discord;
        }

        private IEnumerable<TwitterSubscription> GetSubscriptions()
        {
            return _db.TwitterSubscription
                .Include(x => x.SubscriptionInfo)
                .Where(x => x.SubscriptionInfo.DeletedAt == null && x.DeletedAt == null)
                .ToList();
        }

        public override async Task Execute(IJobExecutionContext context)
        {
            Log.Information("Checking Twitter...");
            var subscriptions = GetSubscriptions();
            var subscriptionUsers = subscriptions.Select(x => x.Username);
            var tweets = await _twitter.BatchUserRequest(subscriptionUsers);

            foreach (var (username, userTweets) in tweets)
            {
                if (!SeenUsernames.Contains(username.ToLowerInvariant()))
                {
                    continue;
                }

                Log.Debug($"New tweet from {username}.");
                var tweetSubscriptions = subscriptions.Where(x => string.Equals(x.Username, username, StringComparison.InvariantCultureIgnoreCase));
                foreach (var tweetSubscription in tweetSubscriptions)
                {
                    var channel = _discord.GetChannel(tweetSubscription.SubscriptionInfo.Channel);
                    if (channel is ISocketMessageChannel c)
                    {
                        foreach (var tweet in userTweets)
                        {
                            var tweetUrl = TweetUrlTemplate
                                .Replace("{username}", username)
                                .Replace("{id}", tweet.Id);
                            var msg = await c.SendMessageAsync($"New tweet from {username}: {tweetUrl}");
                            if (c is INewsChannel)
                            {
                                await msg.CrosspostAsync();
                            }
                        }
                    }
                }
            }

            foreach (var username in subscriptionUsers)
            {
                if (!SeenUsernames.Contains(username.ToLowerInvariant()))
                {
                    SeenUsernames.Add(username.ToLowerInvariant());
                }
            }
        }
    }
}
