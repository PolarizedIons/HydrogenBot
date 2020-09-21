using System;

namespace HydrogenBot.Database.DbModels
{
    public class TwitchEvent : DbEntity
    {
        public TrackedEvent TrackedEvent { get; set; } = null!;
        public string Streamer { get; set; } = null!;
        public bool Online { get; set; }
    }
}
