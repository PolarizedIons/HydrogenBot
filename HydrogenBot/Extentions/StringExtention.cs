using System.Text.RegularExpressions;

namespace HydrogenBot.Extentions
{
    public static class StringExtention
    {
        private static readonly Regex DiscordCharactersRegex = new Regex("([_~*>`|])", RegexOptions.Multiline);

        public static string EscapeDiscordCharacters(this string input)
        {
            return DiscordCharactersRegex.Replace(input, "\\$1");
        }
    }
}