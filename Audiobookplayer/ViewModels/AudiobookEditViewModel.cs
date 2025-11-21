using Audiobookplayer.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Audiobookplayer.ViewModels;

[QueryProperty(nameof(Audiobook), "Audiobook")]
public partial class AudiobookEditViewModel : ObservableObject
{
	[ObservableProperty]
	private Audiobook audiobook;

    public AudiobookEditViewModel()
	{
		
	}


}