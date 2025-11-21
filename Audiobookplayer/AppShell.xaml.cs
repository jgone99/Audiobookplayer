using Audiobookplayer.Pages;

namespace Audiobookplayer
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Application.Current.UserAppTheme = AppTheme.Light;
            Routing.RegisterRoute("EditPage", typeof(AudibookEditPage));
        }
    }
}
