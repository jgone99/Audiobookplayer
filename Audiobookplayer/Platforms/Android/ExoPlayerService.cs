

using Android.App;
using Android.Content;
using AndroidX.Media3.ExoPlayer;
using AndroidX.Media3.Session;

namespace Audiobookplayer.Platforms.Android
{
    public class ExoPlayerService : MediaSessionService
    {
        private IExoPlayer _player;
        private MediaSession _mediaSession;
        private NotificationManager notificationManager;

        public override void OnCreate()
        {
            base.OnCreate();

            NotificationChannel channel = new NotificationChannel("AudiobookPlayerChannel", "Bookmark", NotificationImportance.Default);
            channel.Description = "Bookmark channel for foreground service notification";

            notificationManager = GetSystemService(Java.Lang.Class.FromType(typeof(NotificationManager))) as NotificationManager;
            notificationManager.CreateNotificationChannel(channel);

            Notification notification = new Notification.Builder(this, channel.Id)
                .SetContentTitle("Playing...")
                .SetContentText($"No audiobook selected")
                .SetSmallIcon(Resource.Drawable.dotnet_bot)
                .SetAutoCancel(false)
                .SetOngoing(false)
                .Build();
            StartForeground(1, notification);
            _player = new ExoPlayerBuilder(Platform.AppContext).Build() ?? throw new InvalidOperationException("Failed to create ExoPlayer instance");
            _mediaSession = new MediaSession.Builder(Platform.AppContext, _player).Build() ?? throw new InvalidOperationException("Failed to create MediaSession instance");
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _player.Release();
            _mediaSession.Release();
            _player = null;
            _mediaSession = null;
            base.OnDestroy();
        }

        public override MediaSession? OnGetSession(MediaSession.ControllerInfo? p0)
        {
            return _mediaSession;
        }
    }
}
