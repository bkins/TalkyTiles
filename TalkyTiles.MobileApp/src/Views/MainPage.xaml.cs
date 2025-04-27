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

        await ViewModel.InitializeAsync();

        // if (ViewModel.Buttons.Any().Not())
        // {
        //     await ViewModel.Canvas.LoadPageAsync("first‐page‐id");
        // }
    }

}
