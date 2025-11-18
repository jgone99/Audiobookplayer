namespace Audiobookplayer
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            string libraryFolder = Preferences.Default.Get("library_folder", "Unknown");
            if (libraryFolder == "Unknown")
            {
                Directory.CreateDirectory(Path.Combine(FileSystem.AppDataDirectory, "audiobooks"));
                Preferences.Default.Set("library_folder", Path.Combine(FileSystem.AppDataDirectory, "audiobooks"));
            }
            else if (!Directory.Exists(libraryFolder))
            {
                Directory.CreateDirectory(Path.Combine(FileSystem.AppDataDirectory, "audiobooks"));
            }
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}