
namespace Audiobookplayer.Models
{
    public class PlaybackProgress
    {
        public string? AudiobookId { get; set; }
        public double Position { get; set; }
        public double Duration { get; set; }

        public DateTime LastUpdated { get; set; }
        public bool IsCompleted { get; set; }
    }
}
