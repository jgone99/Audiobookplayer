
namespace Audiobookplayer.Services
{
    internal abstract class FolderPickerService
    {
        public abstract Task<string?> PickFolderAsync(string preferenceKey);
        public static string? GetSavedLibraryFolder(string key)
        {
            var value = Preferences.Default.Get(key, string.Empty);
            return string.IsNullOrEmpty(value) ? null : value;
        }

        public static void SaveLibraryFolder(string key, string folderPath)
        {
            Preferences.Default.Set(key, folderPath);
        }
    }
}
