
namespace Audiobookplayer.Models
{
    public class Audiobook
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Narrator { get; set; }
        public TimeSpan Duration { get; set; }
        public string FilePath { get; set; }
        public ImageSource CoverImage { get; set; }
    }
}
 