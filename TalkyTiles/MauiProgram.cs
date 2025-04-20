using Microsoft.Extensions.Logging;
using TalkyTiles.Services;
using TalkyTiles.Services.Interfaces;
using TalkyTiles.ViewModels;
using TalkyTiles.Views;

namespace TalkyTiles
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<IAudioService, AudioService>();
            builder.Services.AddSingleton<StorageService>();

            builder.Services.AddTransient<MainPageViewModel>();
            builder.Services.AddTransient<MainPage>();
#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
