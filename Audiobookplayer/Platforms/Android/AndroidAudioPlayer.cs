
using Audiobookplayer.Services;
using Android.Content;
using AndroidX.Media3.Common;
using Uri = Android.Net.Uri;
using AndroidX.Media3.Session;
using Google.Common.Util.Concurrent;
using Bumptech.Glide.Util;

namespace Audiobookplayer.Platforms.Android
{
    public class AndroidAudioPlayer : IAudioPlayer
    {
        private IListenableFuture _controllerFuture;
        private MediaController _controller;
        
        public long Duration => _controller.Duration;
        public long CurrentPosition => _controller.CurrentPosition;
        public bool IsPlaying => _controller.IsPlaying;

        public event EventHandler PlaybackStateChanged;
        public event Action<bool>? IsPlayingChanged;

        public AndroidAudioPlayer()
        {
            try
            {
                var name = Java.Lang.Class.FromType(typeof(ExoPlayerService));
                ComponentName componentName = new(Platform.AppContext, Java.Lang.Class.FromType(typeof(ExoPlayerService)));
                SessionToken sessionToken = new(Platform.AppContext, componentName);
                _controllerFuture = new MediaController.Builder(Platform.AppContext, sessionToken)
                    .BuildAsync() ?? throw new InvalidOperationException("Failed to create MediaController instance");
                _controllerFuture.AddListener(new Java.Lang.Runnable(() =>
                {
                    _controller = (MediaController)_controllerFuture.Get();
                    CustomPlayerListener listener = new();
                    listener.IsPlayingChanged += (isPlaying) => IsPlayingChanged.Invoke(isPlaying);
                    (_controller as IPlayer).AddListener(listener);
                    _controller.SetPlaybackSpeed(1.0f);
                }), Executors.MainThreadExecutor());

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating SessionToken: {ex.Message}");
                throw;
            }
        }

        public void LoadAudio(string filePath)
        {
            var mediaItem = MediaItem.FromUri(Uri.Parse(filePath));
            _controller.SetMediaItem(mediaItem);
            _controller.Prepare();
        }

        public void Pause() => _controller.Pause();
        public void Play() => _controller.Play();

        public void SeekTo(long position) => _controller.SeekTo(position);

        public void SetPlaybackSpeed(float speed) => _controller.SetPlaybackSpeed(speed);
    }
}