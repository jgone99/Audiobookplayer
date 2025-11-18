using Android.App;
using Android.Content;
using Android.Content.PM;
using Audiobookplayer.Platforms.Android;

namespace Audiobookplayer
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == 1001)
            {
                if (resultCode == Result.Ok && data?.Data != null)
                    FolderPickerServiceAndriod.OnActivityResult(requestCode, resultCode, data);
                else
                    FolderPickerServiceAndriod.OnActivityResult(requestCode, resultCode, null);
            }
        }
    }
}
