using System;
namespace CS386_Project.code
{

    public enum Source
    {
        UNDEFINED,
        YouTube,
        Spotify
    }

    public class Song
    {
        public Song()
        {
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime RequestTimeStamp { get; set; }
        public Location Location { get; set; }
        public bool HasBeenDownloaded { get; set; }
        public Source Source { get; set; }
    }
}
