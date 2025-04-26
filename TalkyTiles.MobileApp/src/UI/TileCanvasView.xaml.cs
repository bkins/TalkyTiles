using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Layouts;
using TalkyTiles.Core.ViewModels;

namespace TalkyTiles.MobileApp.UI;

public partial class TileCanvasView : ContentView
{
    public TileCanvasView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(
                    nameof(ItemsSource)
                  , typeof(ObservableCollection<SoundButtonViewModel>)
                  , typeof(TileCanvasView)
                  , propertyChanged: OnItemsSourceChanged);

    public ObservableCollection<SoundButtonViewModel> ItemsSource
    {
        get => (ObservableCollection<SoundButtonViewModel>)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty
                      , value);
    }

    private static void OnItemsSourceChanged (BindableObject bindable
                                            , object         oldValue
                                            , object         newValue)
    {
        if (bindable is not TileCanvasView view) return;

        if (oldValue is INotifyCollectionChanged oldCollection)
            oldCollection.CollectionChanged -= view.OnCollectionChanged;

        if (newValue is ObservableCollection<SoundButtonViewModel> newCollection)
        {
            newCollection.CollectionChanged += view.OnCollectionChanged;
            view.RenderTiles();
        }
    }

    private void OnCollectionChanged (object                           sender
                                    , NotifyCollectionChangedEventArgs e)
    {
        RenderTiles();
    }

    private void RenderTiles()
    {
        Canvas.Children.Clear();

        if (ItemsSource == null) return;

        foreach (var buttonVm in ItemsSource)
        {
            var tile = new Views.TileButtonView
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

            Canvas.Children.Add(tile);
        }
    }
}
