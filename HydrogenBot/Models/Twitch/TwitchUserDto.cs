using System.Text.Json.Serialization;

namespace HydrogenBot.Models.Twitch
{
    public class TwitchUserDto
    {
        [JsonPropertyName("_id")]
        public string InternalId { get; set; } = "0";

        public uint Id => uint.Parse(InternalId);

        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; } = null!;
    }
}
