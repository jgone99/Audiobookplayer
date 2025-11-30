using Audiobookplayer.Models;

namespace Audiobookplayer.Services
{
    public class PlayerService(IAudioPlayer player)
    {
        private IAudioPlayer _player = player;
        public bool IsPlaying => _player.IsPlaying;
        public Audiobook? CurrentAudiobook { get; private set; }
        
        public event Action<bool>? IsPlayingChanged;
        public event Action<Audiobook?>? OnAudiobookChanged;

        public async Task SetBookAsync(Audiobook? audiobook)
        {
            if (CurrentAudiobook != null && audiobook.FilePath == CurrentAudiobook.FilePath)
                return;
            OnAudiobookChanged?.Invoke(audiobook);
            CurrentAudiobook = audiobook;
            _player.IsPlayingChanged += (isPlaying) => IsPlayingChanged.Invoke(isPlaying);
        }

        public void LoadAudio(string filePath) => _player.LoadAudio(filePath);

        public void Pause() 
        {
            _player.Pause();
        }
        public void Play()
        {
            _player.Play();
        }
        public long GetCurrentPosition()
        {
            return _player.CurrentPosition;
        }

        public long GetDuration()
        {
            return _player.Duration;
        }

        public void SeekTo(long position) => _player.SeekTo(position);

        public void SetPlaybackSpeed(float speed) => _player.SetPlaybackSpeed(speed);
    }
}
