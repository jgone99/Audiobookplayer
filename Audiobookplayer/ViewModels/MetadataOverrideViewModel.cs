using Audiobookplayer.Models;
using Audiobookplayer.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace Audiobookplayer.ViewModels;

public partial class MetadataOverrideViewModel : ObservableObject, IQueryAttributable
{
	[ObservableProperty]
	private string title;
	[ObservableProperty]
	private string author;
	[ObservableProperty]
	private string narrator;
    private Audiobook audiobook;
    public ICommand SaveMetadataCommand { private set; get; }
    public ICommand ResetMetadataCommand { private set; get; }
    public MetadataOverrideViewModel()
	{
		SaveMetadataCommand = new AsyncRelayCommand(SaveMetadata);
        ResetMetadataCommand = new AsyncRelayCommand(ResetMetadata);
    }

    private async Task SaveMetadata()
	{
        MetadataOverrideService.SaveOverrides(audiobook.Id, new MetadataOverride
        {
            Title = Title,
            Author = Author,
            Narrator = Narrator
        });
        FileSystemServices.NotifyLibraryFolderChanged();
        await Shell.Current.GoToAsync("..");
	}

    private async Task ResetMetadata()
    {
        MetadataOverrideService.DeleteOverrides(audiobook.FilePath);
        FileSystemServices.NotifyLibraryFolderChanged();
        await Shell.Current.GoToAsync("..");
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Audiobook", out var obj) && obj is Audiobook book)
        {
            Title = book.Title;
            Author = book.Author;
            Narrator = book.Narrator;
            audiobook = book;
        }
    }
}