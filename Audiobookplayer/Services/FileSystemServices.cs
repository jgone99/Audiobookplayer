

using Audiobookplayer.Models;

#if ANDROID
using Android.Media;
using Android.Provider;
using Android.Net;
using Android.App;
using Android.Content;
#endif

namespace Audiobookplayer.Services
{
    public static class FileSystemServices
    {
        internal static FolderPickerService FolderPickerService { get; private set; }
        public static event Action? OnLibraryFolderChanged;

        static FileSystemServices()
        {
#if ANDROID
            FolderPickerService = new Platforms.Android.FolderPickerServiceAndriod();
#endif
        }

        public static async Task<List<Audiobook>> LoadAudiobooksFromUriAsync(string? libraryUriString)
        {
#if ANDROID
            var libraryUri = Android.Net.Uri.Parse(libraryUriString);
            if (libraryUri != null)
                return await Platforms.Android.AndroidHelpers.LoadAudiobooksFromUriAsync(libraryUri);
#endif
            return [];
        }

        public static string? GetDisplayPath(string? libraryUriString)
        {
#if ANDROID
            var libraryUri = Android.Net.Uri.Parse(libraryUriString);
            if (libraryUri != null)
                return Platforms.Android.AndroidHelpers.GetDisplayPath(libraryUriString);
#endif
            return string.Empty;
        }

        public static void NotifyLibraryFolderChanged()
        {
            OnLibraryFolderChanged?.Invoke();
        }
    }
}