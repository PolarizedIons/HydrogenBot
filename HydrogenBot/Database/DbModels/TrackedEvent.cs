namespace HydrogenBot.Database.DbModels
{
    public class TrackedEvent : DbEntity
    {
        public ulong Channel { get; set; }
        public string? MentionString { get; set; }
    }
}
