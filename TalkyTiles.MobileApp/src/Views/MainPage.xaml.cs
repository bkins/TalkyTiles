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
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (ViewModel.Buttons.Any().Not())
        {
            await ViewModel.InitializeAsync();
        }
    }
}


// using System.Collections.Specialized;
// using Microsoft.Maui.Layouts;
// using TalkyTiles.MobileApp.Services;
// using TalkyTiles.MobileApp.ViewModels;
//
// namespace TalkyTiles.MobileApp.Views;
//
// public partial class MainPage : ContentPage
// {
//     private MainPageViewModel ViewModel => (MainPageViewModel)BindingContext;
//
//     public MainPage()
//     {
//         InitializeComponent();
//         BindingContext = AppServices.Get<MainPageViewModel>();
//     }
//
//     protected override async void OnAppearing()
//     {
//         base.OnAppearing();
//
//         if (!ViewModel.Buttons.Any())
//         {
//             await ViewModel.InitializeAsync();
//         }
//
//         LoadTiles();
//
//         ViewModel.Buttons.CollectionChanged += (_, _) => LoadTiles();
//     }
//
//     private void LoadTiles()
//     {
//         TileCanvas.Children.Clear();
//
//         foreach (var button in ViewModel.Buttons)
//         {
//             var tile = new TileButtonView { BindingContext = button };
//
//             AbsoluteLayout.SetLayoutBounds(tile, new Rect(button.X, button.Y, 100, 100));
//             AbsoluteLayout.SetLayoutFlags(tile, AbsoluteLayoutFlags.None);
//
//             TileCanvas.Children.Add(tile);
//         }
//     }
// }
