using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using HydrogenBot.Commands;
using HydrogenBot.Models;

namespace HydrogenBot.TypeReaders
{
    public class NotifyStringTypeReader : TypeReader
    {
        private const string Mentioning = "mentioning";
        private const string When = "when";
        
        public override async Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            var channelTypeReader = new ChannelTypeReader<SocketChannel>();
            var roleTypeReader = new RoleTypeReader<SocketRole>();

            var inputQueue = new Queue<string>(input.Split(" "));
            var channelStr = inputQueue.Dequeue();
            var channelReaderResult = await channelTypeReader.ReadAsync(context, channelStr, services);
            if (!channelReaderResult.IsSuccess)
            {
                return TypeReaderResult.FromError(CommandError.ParseFailed, "Couldn't understand the channel; Usage: " + ManageSubscriptions.Usage);
            }

            SocketChannel channel = (SocketChannel) channelReaderResult.BestMatch;

            SocketRole? mention = null;
            if (inputQueue.Count > 0 && inputQueue.Peek() == Mentioning)
            {
                inputQueue.Dequeue();
                var mentionReaderResult = await roleTypeReader.ReadAsync(context, inputQueue.Dequeue(), services);
                if (!mentionReaderResult.IsSuccess)
                {
                    return TypeReaderResult.FromError(CommandError.ParseFailed, "Couldn't understand the role; Usage: " + ManageSubscriptions.Usage);
                }

                mention = (SocketRole) mentionReaderResult.BestMatch;
            }

            if (inputQueue.Count == 0 || inputQueue.Peek() != When)
            {
                return TypeReaderResult.FromError(CommandError.ParseFailed, "Must have a 'when'; Usage: " + ManageSubscriptions.Usage);
            }
            inputQueue.Dequeue();

            if (inputQueue.Count == 0)
            {
                return TypeReaderResult.FromError(CommandError.ParseFailed, "Must have a service; Usage: " + ManageSubscriptions.Usage);
            }

            var service = inputQueue.Dequeue();
            var config = string.Join(" ", inputQueue);

            return TypeReaderResult.FromSuccess(new NotifyString()
            {
                ChannelId = channel.Id,
                MentionString = mention?.Mention,
                ServiceId = service,
                ServiceConfig = config
            });
        }
    }
}
