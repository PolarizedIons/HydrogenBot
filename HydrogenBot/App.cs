using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using HydrogenBot.Database;
using HydrogenBot.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace HydrogenBot
{
    public class App : IHostedService
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IConfiguration _config;
        private readonly DiscordSocketClient _discord;

        private readonly CommandHandler _commandHandler;

        public App(DatabaseContext databaseContext, IConfiguration config, DiscordSocketClient discord, IServiceProvider services)
        {
            _databaseContext = databaseContext;
            _config = config;
            _discord = discord;

            // Init required services
            services.GetRequiredService<BotLogger>();
            _commandHandler = services.GetRequiredService<CommandHandler>();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Log.Information("Migrating Database...");
            await _databaseContext.Database.MigrateAsync(cancellationToken: cancellationToken);

            Log.Debug("Initializing command handler");
            await _commandHandler.InitializeAsync();

            Log.Information("Logging in...");
            await _discord.LoginAsync(TokenType.Bot, _config["Bot:Token"]);
            await _discord.StartAsync();
            Log.Information("Bot started");

            await Task.Delay(-1, cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _discord.StopAsync();
            await _discord.LogoutAsync();
        }
    }
}
