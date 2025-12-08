
namespace Audiobookplayer.Services
{
    public interface IAudioPlayer
    {
        event Action<bool>? IsPlayingChanged;
        void LoadAudio(string filePath);
        void Play();
        void Pause();
        void SeekTo(long position);
        long Duration { get; }
        long CurrentPosition { get; }
        bool IsPlaying { get; }

        void SetPlaybackSpeed(float speed);

        int GetPlaybackState();

        event EventHandler PlaybackStateChanged;
        public event Action? IsReady;
    }
}
