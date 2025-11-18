using Audiobookplayer.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Windows.Input;
namespace Audiobookplayer.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private const string LibraryPrefKey = "library_uri";

        public string LibraryFolderDisplayPath
        {
            get => libraryFolderPath;
            set
            {
                SetProperty(ref libraryFolderPath, value);
            }
        }

        public ICommand FolderPickerCommand { private set; get; }

        private string libraryFolderPath;

        public SettingsViewModel()
        {
            string? libraryFolderUriString = FolderPickerService.GetSavedLibraryFolder(LibraryPrefKey);
            LibraryFolderDisplayPath = $"\"{FileSystemServices.GetDisplayPath(libraryFolderUriString) ?? string.Empty}\"";
            FolderPickerCommand = new AsyncRelayCommand(SelectLibraryFolderAsync);
        }

        private async Task SelectLibraryFolderAsync()
        {
            string? uriString = await FileSystemServices.FolderPickerService.PickFolderAsync(LibraryPrefKey);

            if (!string.IsNullOrEmpty(uriString))
            {
                FolderPickerService.SaveLibraryFolder(LibraryPrefKey, uriString);
                LibraryFolderDisplayPath =
                    $"\"{FileSystemServices.GetDisplayPath(uriString) ?? string.Empty}\"";
                FileSystemServices.NotifyLibraryFolderChanged();
            }
        }
    }
}
