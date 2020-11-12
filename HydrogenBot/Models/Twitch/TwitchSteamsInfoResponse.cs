using System.Text.Json.Serialization;

namespace HydrogenBot.Models.Twitch
{
    public struct TwitchSteamsInfoResponse
    {
        [JsonPropertyName("streams")]
        public TwitchStreamInfo[] Streams { get; set; }
    }

    public struct TwitchStreamInfo
    {
        [JsonPropertyName("game")]
        public string Game { get; set; }

        [JsonPropertyName("channel")]
        public TwitchStreamChannel Channel { get; set; }
    }

    public struct TwitchStreamChannel
    {
        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("_id")]
        public uint Id { get; set; }
    }
}
