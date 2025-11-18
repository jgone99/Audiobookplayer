using Android.Content;
using Android.App;
using Uri = Android.Net.Uri;
using Application = Android.App.Application;

namespace Audiobookplayer.Platforms.Android
{
    internal class FolderPickerServiceAndriod : Services.FolderPickerService
    {
        private static TaskCompletionSource<Uri?>? _tcs;
        public override async Task<string?> PickFolderAsync(string preferenceKey)
        {
            try
            {
                var activity = Platform.CurrentActivity ?? throw new InvalidOperationException("No current activity");

                _tcs = new TaskCompletionSource<Uri?>();

                // Create intent to open folder picker
                Intent intent = new Intent(Intent.ActionOpenDocumentTree);
                intent.AddFlags(ActivityFlags.GrantPersistableUriPermission |
                                ActivityFlags.GrantReadUriPermission |
                                ActivityFlags.GrantWriteUriPermission);

                // Start activity
                activity.StartActivityForResult(intent, 1001);

                Uri? uri = await _tcs.Task;

                if (uri != null)
                {
                    var resolver = Application.Context.ContentResolver;

                    resolver.TakePersistableUriPermission(
                        uri,
                        ActivityFlags.GrantReadUriPermission | ActivityFlags.GrantWriteUriPermission
                    );

                    // Persist on main thread
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Preferences.Default.Set(preferenceKey, uri.ToString());
                    });
                }

                return uri == null ? GetSavedLibraryFolder(preferenceKey) : uri.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error picking folder: {ex.Message}");
                return null;
            }
        }

        public static void OnActivityResult(int requestCode, Result resultCode, Intent? data)
        {
            if (requestCode != 1001) return;

            if (resultCode == Result.Ok && data?.Data != null)
            {
                _tcs?.TrySetResult(data.Data);
            }
            else
            {
                _tcs?.TrySetResult(null);
            }
        }
    }
}