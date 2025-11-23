
using Audiobookplayer.Services;
using Android.Content;
using Android.App;
using Android.Net;
using AndroidX.Media3.Common;
using AndroidX.Media3.ExoPlayer;
using Application = Android.App.Application;
using Uri = Android.Net.Uri;

namespace Audiobookplayer.Platforms.Android
{
    public class ExoPlayerService : IAudioPlayer
    {
        private IExoPlayer _player;
        public long Duration => _player.Duration;

        public long CurrentPosition => _player.CurrentPosition;

        public event EventHandler PlaybackStateChanged;

        public ExoPlayerService()
        {
            _player = new ExoPlayerBuilder(Platform.AppContext).Build() ?? throw new InvalidOperationException("Failed to create ExoPlayer instance");
        }

        public async Task LoadAsync(string filePath)
        {
            var mediaItem = MediaItem.FromUri(Uri.Parse(filePath));
            _player.SetMediaItem(mediaItem);
            _player.Prepare();
        }

        public void Pause() => _player.Pause();
        public void Play() => _player.Play();

        public void SeekTo(long position) => _player.SeekTo(position);
    }
}