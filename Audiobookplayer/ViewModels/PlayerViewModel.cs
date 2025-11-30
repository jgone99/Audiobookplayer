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
        private double position = 0.0;

        [ObservableProperty]
        private bool isDragging = false;

        [ObservableProperty]
        private bool isDoubleSpeed = false;

        [ObservableProperty]
        private string playPauseIcon = "pause.png";

        private Audiobook? currentBook;

        private bool savedStateWasPlaying;
        

        public ICommand PausePlayCommand { private set; get; }
        public ICommand DragCompleteCommand { private set; get; }
        public ICommand DragStartCommand { private set; get; }
        public ICommand DoubleSpeedCommand { private set; get; }

        IDispatcherTimer seekbarTrackingTimer = Dispatcher.GetForCurrentThread().CreateTimer();

        public PlayerViewModel()
        {
            _playerService = ((App)App.Current).Services.GetService<PlayerService>() ?? throw new InvalidOperationException("PlayerService not found");
            _playerService.OnAudiobookChanged += OnBookChanged;
            _playerService.IsPlayingChanged += OnPlayingChanged;

            PausePlayCommand = new AsyncRelayCommand(PausePlayToggle);
            DragCompleteCommand = new RelayCommand(OnDragComplete);
            DragStartCommand = new RelayCommand(OnDragStart);
            DoubleSpeedCommand = new AsyncRelayCommand(DoubleSpeedToggle);

            currentBook = _playerService.CurrentAudiobook;
            BookTitle = currentBook?.Title ?? "No book loaded";
            CoverImage = currentBook?.CoverImage;
            LoadAudiobookAsync();
            SetToPlay();

            seekbarTrackingTimer.Interval = TimeSpan.FromMilliseconds(250);
            seekbarTrackingTimer.Tick += UpdateSeekbarPosition;
            seekbarTrackingTimer.IsRepeating = true;

            seekbarTrackingTimer.Start();
        }

        private void UpdateSeekbarPosition(object? sender, EventArgs? args)
        {
            var pos = _playerService.GetCurrentPosition();
            var dur = _playerService.GetDuration();
            Debug.WriteLine($"pos = {pos / currentBook.Duration.TotalMilliseconds} \nPosition = {Position}");
            Position = _playerService.GetCurrentPosition() / currentBook.Duration.TotalMilliseconds;
            
        }

        private async void OnBookChanged(Audiobook? book)
        {
            ResetView();
            currentBook = book;
            BookTitle = book?.Title ?? "No book loaded";
            CoverImage = book?.CoverImage;
            LoadAudiobookAsync();    
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

        private void LoadAudiobookAsync()
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
                _playerService.SetPlaybackSpeed(1.0f);
            else
                _playerService.SetPlaybackSpeed(2.0f);
            IsDoubleSpeed = !IsDoubleSpeed;
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
            SetToPlay();
        }

        private void SeekTo(double position)
        {
            var dur = _playerService.GetDuration();
            long pos = (long)(position * currentBook.Duration.TotalMilliseconds);
            _playerService.SeekTo(pos);
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
    }
}
