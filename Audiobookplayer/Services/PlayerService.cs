using Audiobookplayer.Models;

namespace Audiobookplayer.Services
{
    public class PlayerService
    {
        private IAudioPlayer _player;
        public bool IsPlaying => _player.IsPlaying;
        public Audiobook? CurrentAudiobook { get; private set; }
        
        public event Action<bool>? IsPlayingChanged;
        public event Action? IsReady;
        public event Action<Audiobook?>? OnAudiobookChanged;

        public PlayerService(IAudioPlayer player)
        {
            _player = player;
            _player.IsPlayingChanged += (isPlaying) => IsPlayingChanged.Invoke(isPlaying);
            _player.IsReady += () => IsReady.Invoke();
        }

        public async Task SetBookAsync(Audiobook? audiobook)
        {
            if (CurrentAudiobook != null && audiobook.FilePath == CurrentAudiobook.FilePath)
                return;
            CurrentAudiobook = audiobook;
            OnAudiobookChanged?.Invoke(audiobook);
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

        public int GetPlaybackState()
        {
            return _player.GetPlaybackState();
        }
    }
}
