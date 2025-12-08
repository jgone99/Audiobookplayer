using Audiobookplayer.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Audiobookplayer.Services;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace Audiobookplayer.ViewModels
{
    public partial class LibraryViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<Audiobook> audiobooks = new();

        [ObservableProperty]
        private bool inEditMode;

        private readonly string libraryPrefKey = "library_uri";

        private string? libraryUriString;

        public ICommand SelectAudiobookCommand { private set; get; }
        public ICommand DeleteAudiobookCommand { private set; get; }
        public ICommand ToggleEditModeCommand { private set; get; }
        public ICommand EditAudiobookCommand { private set; get; }

        private PlayerService _playerService;

        public LibraryViewModel()
        {
            _playerService = ((App)App.Current).Services.GetService<PlayerService>() ?? throw new InvalidOperationException("PlayerService not found");
            FileSystemServices.OnLibraryFolderChanged += OnLibraryFolderChanged;

            SelectAudiobookCommand = new AsyncRelayCommand<Audiobook>(SelectAudiobookAsync);
            DeleteAudiobookCommand = new AsyncRelayCommand<Audiobook>(DeleteAudiobook);
            ToggleEditModeCommand = new RelayCommand(ToggleEditMode);
            EditAudiobookCommand = new AsyncRelayCommand<Audiobook>(EditAudiobook);

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

                loadedAudiobooks.Sort((book1, book2) => { return book1.Title.CompareTo(book2.Title); });

                foreach (var audiobook in loadedAudiobooks)
                {
                    audiobook.Id = GenerateAudiobookId(audiobook.FilePath);

                    MetadataOverride overrides = MetadataOverrideService.GetOverrides(audiobook.Id);
                    if (overrides != null)
                    {
                        audiobook.Title = overrides.Title;
                        audiobook.Author = overrides.Author;
                        audiobook.Narrator = overrides.Narrator;
                    }
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

        private async Task SelectAudiobookAsync(Audiobook? audiobook)
        {
            if (audiobook == null)
                return;
            await _playerService.SetBookAsync(audiobook);

            await Task.Yield();
            await Task.Delay(50);

            await Shell.Current.GoToAsync("//PlayerTab", true);
        }

        private async Task DeleteAudiobook(Audiobook? audiobook)
        {
            await Task.Yield();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                Audiobooks.Remove(audiobook);
            });

            FileSystemServices.DeleteFile(audiobook.FilePath);
        }

        private void ToggleEditMode()
        {
            InEditMode = !InEditMode;
        }

        private async Task EditAudiobook(Audiobook audiobook)
        {
            if (audiobook == null)
                return;
            await Shell.Current.GoToAsync("EditPage", true, new Dictionary<string, object>
            {
                { "Audiobook", audiobook }
            });
        }

        private static string GenerateAudiobookId(string filepath)
        {
            using Stream stream = FileSystemServices.OpenInputStream(filepath);
            using System.Security.Cryptography.SHA256 sha256 = System.Security.Cryptography.SHA256.Create();

            var buff = new byte[10 * 1024 * 1024];

            int bytesRead = stream.Read(buff, 0, buff.Length);

            byte[] hash = sha256.ComputeHash(buff, 0, bytesRead);
            return Convert.ToHexString(hash);
        }
    }
}
