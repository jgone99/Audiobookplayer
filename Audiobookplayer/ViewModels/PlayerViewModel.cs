using Audiobookplayer.Models;
using Audiobookplayer.Services;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;
using Microsoft.Maui.Controls.Shapes;
using System.Diagnostics;

namespace Audiobookplayer.ViewModels
{
    public partial class PlayerViewModel : ObservableObject
    {
        private readonly PlayerService _playerService;
        
        [ObservableProperty]
        private bool isPlaying = true;

        [ObservableProperty]
        private string pausePlayButtonIcon;

        [ObservableProperty]
        private string bookTitle = "No book loaded";

        [ObservableProperty]
        private ImageSource coverImage;

        [ObservableProperty]
        private RectangleGeometry coverImageRect;

        [ObservableProperty]
        private double position;

        [ObservableProperty]
        private double duration;

        [ObservableProperty]
        private bool isDoubleSpeed = false;

        [ObservableProperty]
        private string playPauseIcon = "pause.png";

        [ObservableProperty]
        private bool bookSelected = false;

        private Audiobook? currentBook;

        private bool savedStateWasPlaying;

        private PlaybackProgress? progress;

        public ICommand PausePlayCommand { private set; get; }
        public ICommand DragCompleteCommand { private set; get; }
        public ICommand DragStartCommand { private set; get; }
        public ICommand DoubleSpeedCommand { private set; get; }
        public ICommand SkipForwardCommand { private set; get; }
        public ICommand SkipBackwardCommand { private set; get; }

        IDispatcherTimer seekbarTrackingTimer = Dispatcher.GetForCurrentThread().CreateTimer();

        public PlayerViewModel()
        {
            _playerService = ((App)App.Current).Services.GetService<PlayerService>() ?? throw new InvalidOperationException("PlayerService not found");
            _playerService.OnAudiobookChanged += OnBookChanged;
            _playerService.IsPlayingChanged += OnPlayingChanged;
            InitPlayer();
        }

        private void InitPlayer()
        {
            PausePlayCommand = new AsyncRelayCommand(PausePlayToggle);
            DragCompleteCommand = new RelayCommand(OnDragComplete);
            DragStartCommand = new RelayCommand(OnDragStart);
            DoubleSpeedCommand = new AsyncRelayCommand(DoubleSpeedToggle);
            SkipBackwardCommand = new AsyncRelayCommand(SkipBackward);
            SkipForwardCommand = new AsyncRelayCommand(SkipForward);

            seekbarTrackingTimer.Interval = TimeSpan.FromMilliseconds(250);
            seekbarTrackingTimer.Tick += UpdateSeekbarPosition;
            seekbarTrackingTimer.IsRepeating = true;

            BookSelected = _playerService.CurrentAudiobook != null;

            if (BookSelected)
            {
                OnBookChanged(_playerService.CurrentAudiobook);
                SetToPlay();
                seekbarTrackingTimer.Start();
            }
        }

        private void UpdateSeekbarPosition(object? sender = null, EventArgs? args = null)
        {
            Debug.WriteLine($"Position = {Position}");
            Position = (double)_playerService.GetCurrentPosition();
            progress.Position = Position;
            PlaybackProgressService.Save(currentBook.Id, progress);
        }

        private async void OnBookChanged(Audiobook? book)
        {
            BookSelected = true;
            currentBook = book;
            
            progress = PlaybackProgressService.GetProgress(currentBook.Id) ?? new PlaybackProgress();
            
            BookTitle = book?.Title ?? "No book loaded";
            CoverImage = book?.CoverImage;
            Duration = book.Duration.TotalMilliseconds;
            Position = progress?.Position ?? 0.0;
            ResetView();
            LoadAudiobookToPlayerAsync();
            SeekTo(Position);
        }

        private async void OnPlayingChanged(bool isPlaying)
        {
            IsPlaying = isPlaying;
            if (IsPlaying)
            {
                PlayPauseIcon = "pause.png";
                seekbarTrackingTimer.Start();
            } 
            else
            {
                PlayPauseIcon = "play.png";
                seekbarTrackingTimer.Stop();
            }
        }

        private void LoadAudiobookToPlayerAsync()
        {
            if (currentBook == null)
            {
                return;
            }
            Debug.WriteLine("Loading audio");
            _playerService.LoadAudio(currentBook.FilePath);
        }

        private async Task PausePlayToggle()
        {
            if (IsPlaying)
            {
                SetToPause();
            }
            else
            {
                SetToPlay();
            }
        }

        private async Task DoubleSpeedToggle()
        {
            if (IsDoubleSpeed)
                SetDoubleSpeed(false);
            else
                SetDoubleSpeed(true);
        }

        private void SetDoubleSpeed(bool b)
        {
            IsDoubleSpeed = b;
            _playerService.SetPlaybackSpeed(b ? 2.0f : 1.0f);
        }

        private void SetToPlay()
        {
            _playerService.Play();
        }

        private void SetToPause()
        {
            _playerService.Pause();
        }

        private void ResetView()
        {
            SetDoubleSpeed(false);
            SetToPlay();
        }

        private void SeekTo(double newPosition)
        {
            _playerService.SeekTo((long)newPosition);
        }
        public void OnDragComplete()
        {
            SeekTo(Position);
            if (savedStateWasPlaying)
                SetToPlay();
        }

        public void OnDragStart()
        {
            savedStateWasPlaying = IsPlaying;
            if (IsPlaying)
                SetToPause();
        }

        private void SkipTo(double newPosition)
        {
            newPosition = Math.Max(newPosition, 0);
            newPosition = Math.Min(newPosition, Duration);
            savedStateWasPlaying = IsPlaying;
            if (IsPlaying)
                SetToPause();
            SeekTo(newPosition);
            UpdateSeekbarPosition();
            if (savedStateWasPlaying)
                SetToPlay();
        }

        public async Task SkipForward()
        {
            SkipTo(Position + 10000);
        }

        public async Task SkipBackward()
        {
            SkipTo(Position - 10000);
        }
    }
}
