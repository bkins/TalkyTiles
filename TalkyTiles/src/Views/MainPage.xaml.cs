using System.Collections.Specialized;
using Microsoft.Maui.Layouts;
using TalkyTiles.Services;
using TalkyTiles.ViewModels;

namespace TalkyTiles.Views;

public partial class MainPage : ContentPage
{
    private bool _initialized;

    public MainPage()
    {
        InitializeComponent();
        BindingContext = AppServices.Get<MainPageViewModel>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        TileCanvas.Children.Clear();

        var viewModel = (MainPageViewModel)BindingContext;

        if (! _initialized)
        {
            viewModel.Buttons.CollectionChanged += OnButtonsChanged;
            _initialized                        =  true;
        }

        // If no data yet, initialize
        if (! viewModel.Buttons.Any())
        {
            await viewModel.InitializeAsync();
        }

        LoadTiles(viewModel);

    }

    private void OnButtonsChanged (object?                          sender
                                 , NotifyCollectionChangedEventArgs e)
    {
        // We want a fresh render anytime the tile list changes
        var viewModel = (MainPageViewModel)BindingContext;
        LoadTiles(viewModel);
    }

    private void LoadTiles (MainPageViewModel viewModel)
    {
        TileCanvas.Children.Clear();

        foreach (var buttonVm in viewModel.Buttons)
        {
            Console.WriteLine($"Adding tile '{buttonVm.DisplayText}' at ({buttonVm.X}, {buttonVm.Y})");

            var tile = new TileButtonView
                       {
                               BindingContext = buttonVm
                       };

            AbsoluteLayout.SetLayoutBounds(tile
                                         , new Rect(buttonVm.X
                                                  , buttonVm.Y
                                                  , 100
                                                  , 100));
            AbsoluteLayout.SetLayoutFlags(tile
                                        , AbsoluteLayoutFlags.None);

            TileCanvas.Children.Add(tile);
        }
    }
}
