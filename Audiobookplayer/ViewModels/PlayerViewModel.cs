using Audiobookplayer.Models;
using Audiobookplayer.Services;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;

namespace Audiobookplayer.ViewModels
{
    public partial class PlayerViewModel : ObservableObject
    {
        private readonly PlayerService _playerService;
        private bool _isPaused;

        [ObservableProperty]
        private string pausePlayButtonIcon;

        [ObservableProperty]
        private string bookTitle = "No book loaded";

        [ObservableProperty]
        private ImageSource coverImage;

        private Audiobook? currentBook;

        public ICommand PausePlayCommand { private set; get; }

        public PlayerViewModel()
        {
            _playerService = ((App)App.Current).Services.GetService<PlayerService>() ?? throw new InvalidOperationException("PlayerService not found");
            _playerService.OnAudiobookChanged += OnBookChanged;

            PausePlayCommand = new AsyncRelayCommand(PausePlayToggle);

            currentBook = _playerService.CurrentAudiobook;
            BookTitle = currentBook?.Title ?? "No book loaded";
            CoverImage = currentBook?.CoverImage;

            SetPlay();
        }

        private void OnBookChanged(Audiobook? book)
        {
            ResetView();
            currentBook = book;
            BookTitle = book?.Title ?? "No book loaded";
            CoverImage = book?.CoverImage;
            LoadAudio();
            
        }

        private void LoadAudio()
        {
            Console.WriteLine("Loading audio");
            // your audio playback setup
        }

        private async Task PausePlayToggle()
        {
            if (_isPaused)
            {
                SetPlay();
            }
            else
            {
                SetPause();
            }
        }

        private void SetPlay()
        {
            _isPaused = false;
            PausePlayButtonIcon = "pause.png";
            _playerService.Play();
        }

        private void SetPause()
        {
            _isPaused = true;
            PausePlayButtonIcon = "play.png";
            _playerService.Pause();
        }

        private void ResetView()
        {
            SetPlay();
        }
    }
}
