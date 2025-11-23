
namespace Audiobookplayer.Services
{
    public interface IAudioPlayer
    {
        Task LoadAsync(string filePath);
        void Play();
        void Pause();
        void SeekTo(long position);

        long Duration { get; }
        long CurrentPosition { get; }

        event EventHandler PlaybackStateChanged;
    }
}
