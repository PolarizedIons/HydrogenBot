using System.Threading.Tasks;
using Discord.Commands;

namespace HydrogenBot.Commands
{
    public class InfoModule : ModuleBase
    {
        // ~say hello world -> hello world
        [Command("info")]
        [Summary("Gives info about the bot.")]
        public Task GiveInfo() => ReplyAsync("INFOOOOOOOO!");
    }
}
