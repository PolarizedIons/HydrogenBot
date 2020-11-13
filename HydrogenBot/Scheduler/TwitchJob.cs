using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using HydrogenBot.Database;
using HydrogenBot.Extentions;
using HydrogenBot.Models.Twitch;
using HydrogenBot.Services;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Serilog;

namespace HydrogenBot.Scheduler
{
    public class TwitchJob : BaseJob
    {
        public new static int SecondsInterval => 180;

        private readonly DatabaseContext _db;
        private readonly DiscordSocketClient _discord;
        private readonly TwitchService _twitch;

        public TwitchJob(DatabaseContext db, DiscordSocketClient discord, TwitchService twitch)
        {
            _db = db;
            _discord = discord;
            _twitch = twitch;
        }

        public override async Task Execute(IJobExecutionContext context)
        {
            Log.Debug("Checking Twitch for livestreams.");
            var subscriptions = _db.TwitchSubscription.AsQueryable()
                .Where(x => x.DeletedAt == null)
                .Include(x => x.SubscriptionInfo);

            var channelIds = await subscriptions.Select(x => x.StreamerId).ToListAsync();
            var streamInfos = await _twitch.BatchStreamInfo(channelIds);
            var isOnlineCache = streamInfos.ToDictionary(streamInfo => streamInfo.Channel.Id);

            foreach (var subscription in subscriptions)
            {
                var streamInfo = isOnlineCache.ContainsKey(subscription.StreamerId)
                    ? isOnlineCache[subscription.StreamerId]
                    : (TwitchStreamInfo?) null;
                var wasOnline = subscription.Online;
                var isOnlineNow = streamInfo != null;

                if (wasOnline && !isOnlineNow)
                {
                    Log.Debug("Streamer {channelId} is now offline", subscription.StreamerId);
                    subscription.Online = false;
                    continue;
                }

                if (wasOnline || !isOnlineNow)
                {
                    continue;
                }

                subscription.Online = true;
                Log.Debug("Streamer {channelId} is now online", subscription.StreamerId);

                var channel = _discord.GetChannel(subscription.SubscriptionInfo.Channel);
                if (channel is ISocketMessageChannel c)
                {
                    var mentionString = string.IsNullOrEmpty(subscription.SubscriptionInfo.MentionString) ? "" : subscription.SubscriptionInfo.MentionString + ", ";
                    var game = streamInfo?.Game;
                    var name = streamInfo?.Channel.DisplayName.EscapeDiscordCharacters();
                    var url = streamInfo?.Channel.Url;
                    await c.SendMessageAsync($"{mentionString}{name} is now online playing {game} over at {url} !");
                }
            }

            await _db.SaveChangesAsync();
        }
    }
}
