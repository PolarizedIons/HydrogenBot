using System.Text.Json.Serialization;

namespace HydrogenBot.Models.Twitch
{
    public class TwitchSteamInfoResponse
    {
        [JsonPropertyName("stream")]
        public TwitchStreamInfo? Steam { get; set; }

        public bool IsOnline => Steam != null;
    }

    public class TwitchStreamInfo
    {
        [JsonPropertyName("game")]
        public string Game { get; set; } = null!;

        [JsonPropertyName("channel")]
        public TwitchStreamChannel Channel { get; set; } = null!;
    }

    public class TwitchStreamChannel
    {
        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; } = null!;

        [JsonPropertyName("url")]
        public string Url { get; set; } = null!;
    }
}
