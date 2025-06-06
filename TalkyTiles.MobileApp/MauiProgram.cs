﻿using Microsoft.Extensions.Logging;
using TalkyTiles.Core.Services;
using TalkyTiles.Core.Services.Interfaces;
using TalkyTiles.Core.ViewModels;
using TalkyTiles.MobileApp.Views;

namespace TalkyTiles.MobileApp
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
            builder.Services.AddSingleton<ITileStorageService, TileStorageService>();
            builder.Services.AddSingleton<IUiStateService, UiStateService>();
            builder.Services.AddSingleton<StorageService>();

            builder.Services.AddSingleton<MainPageViewModel>();
            builder.Services.AddSingleton<TileCanvasViewModel>();

            builder.Services.AddSingleton<EditTileViewModel>();
            builder.Services.AddSingleton<SoundButtonViewModel>();
            builder.Services.AddSingleton<TileButtonViewModel>();
#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
