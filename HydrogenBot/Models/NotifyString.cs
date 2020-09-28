namespace HydrogenBot.Models
{
    public struct NotifyString
    {
        public string? MentionString { get; set; }
        public ulong ChannelId { get; set; }
        public string ServiceId { get; set; }
        public string ServiceConfig { get; set; }
    }
}
