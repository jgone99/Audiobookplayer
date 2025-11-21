using Audiobookplayer.Services;
using CommunityToolkit.Maui;
using Maui.FreakyEffects;

namespace Audiobookplayer
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .ConfigureEffects(effects =>
                {
                    effects.InitFreakyEffects();
                })
                .Services.AddSingleton<PlayerService>();
            builder.Services.AddSingleton(Plugin.Maui.Audio.AudioManager.Current);
            return builder.Build();
        }
    }
}
