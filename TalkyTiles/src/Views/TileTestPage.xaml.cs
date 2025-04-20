using Microsoft.Maui.Layouts;
using TalkyTiles.Models;
using TalkyTiles.Services;
using TalkyTiles.ViewModels;
using TalkyTiles.Views;

namespace TalkyTiles.Views;

public partial class TileTestPage : ContentPage
{
    public TileTestPage()
    {
        InitializeComponent();

        var model = new TileButtonModel
                    {
                            DisplayText = "Hello"
                          , AudioPath = "/path/to/audio.wav"
                          , X = 50
                          , Y = 100
                    };

        var viewModel = new TileButtonViewModel(model
                                              , new AudioService());
        var tileView = new TileButtonView(viewModel);

        Content = new AbsoluteLayout
                  {
                          Children = { tileView }
                  };

        AbsoluteLayout.SetLayoutBounds(tileView
                                     , new Rect(0
                                              , 0
                                              , 100
                                              , 100));
        AbsoluteLayout.SetLayoutFlags(tileView
                                    , AbsoluteLayoutFlags.None);
    }
}
