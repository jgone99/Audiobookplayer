using Audiobookplayer.Models;

namespace Audiobookplayer.Services
{
    public class PlayerService
    {
        private IAudioPlayer _player;
        public Audiobook? CurrentAudiobook { get; private set; }
        public event Action<Audiobook?>? OnAudiobookChanged;

        public PlayerService(IAudioPlayer player)
        {
            _player = player;
        }

        public async Task SetBookAsync(Audiobook? audiobook)
        {
            OnAudiobookChanged?.Invoke(audiobook);

            CurrentAudiobook = audiobook;

            using Stream stream = FileSystemServices.OpenInputStream(audiobook.FilePath);
            await _player.LoadAsync(audiobook.FilePath);
        }

        public bool hasBook()
        {
            return CurrentAudiobook != null;
        }

        public void Pause() => _player.Pause();
        public void Play() => _player.Play();

        public void SeekTo(long position) => _player.SeekTo(position);
    }
}
