using Audiobookplayer.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Audiobookplayer.Services;
using CommunityToolkit.Mvvm.Messaging;


#if ANDROID
using Android.Provider;
using Uri = Android.Net.Uri;
#endif

namespace Audiobookplayer.ViewModels
{
    public partial class AudiobookViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<Audiobook> audiobooks = new();

        private string libraryPrefKey = "library_uri";

        private string? libraryUriString;

        public AudiobookViewModel()
        {
            FileSystemServices.OnLibraryFolderChanged += OnLibraryFolderChanged;
            LoadAudiobooksAsync();
        }

        private async void LoadAudiobooksAsync()
        {
            Audiobooks.Clear();

            libraryUriString = Preferences.Default.Get(libraryPrefKey, string.Empty);


            try
            {
                if (string.IsNullOrEmpty(libraryUriString))
                {
                    libraryUriString = await FileSystemServices.FolderPickerService.PickFolderAsync(libraryPrefKey);
                }

                var loadedAudiobooks = await FileSystemServices.LoadAudiobooksFromUriAsync(libraryUriString);
                foreach (var audiobook in loadedAudiobooks)
                {
                    Audiobooks.Add(audiobook);
                }

            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log the error)
                Console.WriteLine($"Error loading audiobooks: {ex.Message}");
            }
        }

        private async void OnLibraryFolderChanged()
        {
            await MainThread.InvokeOnMainThreadAsync(() => LoadAudiobooksAsync());
        }
    }
}
