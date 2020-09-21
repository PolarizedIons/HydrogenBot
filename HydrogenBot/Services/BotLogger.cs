using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using HydrogenBot.Models;
using Serilog;

namespace HydrogenBot.Services
{
    public class BotLogger : ISingletonDiService
    {
        public BotLogger(DiscordSocketClient discord)
        {
            discord.Log += log =>
            {
                if (log.Exception != null)
                {
                    Log.Debug(log.Exception, $"[Discord] {log.Source} {log.Message}");
                }
                else
                {
                    Log.Debug($"[Discord] {log.Source} {log.Message}");
                }

                return Task.CompletedTask;
            };

            discord.Ready += () =>
            {
                Log.Information("Discord bot ready!");
                return Task.CompletedTask;
            };

            discord.MessageReceived += message =>
            {
                if (message is SocketUserMessage msg)
                {
                    var guild = (msg.Author as SocketGuildUser)?.Guild;
                    Log.Debug($"[Message] [{msg.Author.Username}#{msg.Author.Discriminator} ({msg.Author.Id})] in ['{guild?.Name}'/#{msg.Channel.Name} ({guild?.Id}/{msg.Channel.Id})]: {msg.Content}");
                }

                return Task.CompletedTask;
            };
        }
    }
}
