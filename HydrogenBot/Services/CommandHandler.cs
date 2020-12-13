using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using HydrogenBot.Models;
using HydrogenBot.TypeReaders;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace HydrogenBot.Services
{
    public class CommandHandler : ISingletonDiService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly IConfiguration _config;
        private readonly IServiceProvider _services;
        
        public CommandHandler(DiscordSocketClient discord, CommandService commands, IConfiguration config, IServiceProvider services)
        {
            _discord = discord;
            _commands = commands;
            _config = config;
            _services = services;
        }

        public async Task InitializeAsync()
        {
            _commands.AddTypeReader(typeof(NotifyString), new NotifyStringTypeReader());

            await _commands.AddModulesAsync(
                assembly: Assembly.GetEntryAssembly(),
                services: _services
            );
            
            _commands.CommandExecuted += OnCommandExecutedAsync;

            _discord.MessageReceived += HandleCommandAsync;
        }

        private async Task HandleCommandAsync(SocketMessage socketMessage)
        {
            if (!(socketMessage is SocketUserMessage msg))
            {
                return;
            }

            var argPos = 0;
            if (!msg.HasStringPrefix(_config["Bot:Prefix"], ref argPos) || msg.Author.IsBot)
            {
                return;
            }

            var context = new CommandContext(_discord, msg);
            await _commands.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: _services
            );
        }

        private async Task OnCommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            // We don't care about unknown commands
            if (result.Error == CommandError.UnknownCommand)
            {
                return;
            }

            if (!string.IsNullOrEmpty(result.ErrorReason))
            {
                await context.Channel.SendMessageAsync("Error: " + result.ErrorReason);
                Log.Error("error {ErrorType}", result.Error);
            }
        }
    }
}
