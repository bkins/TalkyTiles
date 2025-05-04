using Microsoft.Maui.Layouts;
using TalkyTiles.Core.Utilities.Extensions;
using TalkyTiles.Core.ViewModels;
using TalkyTiles.MobileApp.Services;

namespace TalkyTiles.MobileApp.Views;

public partial class MainPage : ContentPage
{
    private MainPageViewModel ViewModel => (MainPageViewModel)BindingContext;

    public MainPage()
    {
        InitializeComponent();
        BindingContext = AppServices.Get<MainPageViewModel>();

        ViewModel.Canvas.Buttons.CollectionChanged += (_, __) => RenderTiles();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await ViewModel.InitializeAsync();
        RenderTiles();

        // if (ViewModel.SelectedPage is not null)
        // {
        //     await ViewModel.Canvas.LoadPageAsync(ViewModel.SelectedPage.Id);
        // }
        // if (ViewModel.Buttons.Any().Not())
        // {
        //     await ViewModel.Canvas.LoadPageAsync("first‐page‐id");
        // }
    }

    void RenderTiles()
    {
        TileCanvas.Children.Clear();

        foreach (var tileVm in ViewModel.Canvas.Buttons)
        {
            var tile = new TileButtonView { BindingContext = tileVm };

            AbsoluteLayout.SetLayoutBounds(tile
                                         , new Rect(tileVm.X
                                                  , tileVm.Y
                                                  , 100
                                                  , 100));
            AbsoluteLayout.SetLayoutFlags(tile
                                        , AbsoluteLayoutFlags.None);

            TileCanvas.Children.Add(tile);
        }
    }

}
