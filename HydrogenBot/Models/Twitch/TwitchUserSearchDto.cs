using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HydrogenBot.Models.Twitch
{
    public class TwitchUserSearchDto
    {
        [JsonPropertyName("users")]
        public IEnumerable<TwitchUserDto> Users { get; set; } = null!;
    }
}