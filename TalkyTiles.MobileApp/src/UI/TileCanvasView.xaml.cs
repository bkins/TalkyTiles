using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.Maui.Layouts;
using TalkyTiles.Core.ViewModels;
using TalkyTiles.MobileApp.Views;

namespace TalkyTiles.MobileApp.UI;

public partial class TileCanvasView : ContentView
{
    public TileCanvasView()
    {
        InitializeComponent();
    }

    // 1) Expose ItemsSource as an ObservableCollection<SoundButtonViewModel>
    public static readonly BindableProperty ItemsSourceProperty =
        BindableProperty.Create(
            nameof(ItemsSource),
            typeof(ObservableCollection<SoundButtonViewModel>),
            typeof(TileCanvasView),
            propertyChanged: OnItemsSourceChanged);

    public ObservableCollection<SoundButtonViewModel>? ItemsSource
    {
        get => (ObservableCollection<SoundButtonViewModel>?)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (TileCanvasView)bindable;

        // Unhook old
        if (oldValue is INotifyCollectionChanged oldCol)
            oldCol.CollectionChanged -= control.OnCollectionChanged;

        // Hook new
        if (newValue is INotifyCollectionChanged newCol)
            newCol.CollectionChanged += control.OnCollectionChanged;

        // Render immediately
        control.RenderTiles();
    }

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        => RenderTiles();

    private void RenderTiles()
    {
        CanvasLayout.Children.Clear();

        if (ItemsSource == null)
            return;

        foreach (var tileVm in ItemsSource)
        {
            var tileView = new TileButtonView
            {
                BindingContext = tileVm
            };

            AbsoluteLayout.SetLayoutBounds(tileView,
                new Rect(tileVm.X, tileVm.Y, 100, 100));
            AbsoluteLayout.SetLayoutFlags(tileView,
                AbsoluteLayoutFlags.None);

            CanvasLayout.Children.Add(tileView);
        }
    }
}


// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Collections.ObjectModel;
// using System.Collections.Specialized;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
// using Microsoft.Maui.Layouts;
// using TalkyTiles.Core.ViewModels;
// using TalkyTiles.MobileApp.Views;
//
// namespace TalkyTiles.MobileApp.UI;
//
// public partial class TileCanvasView : ContentView
// {
//     public ObservableCollection<SoundButtonViewModel> ItemsSource
//     {
//         get => (ObservableCollection<SoundButtonViewModel>)GetValue(ItemsSourceProperty);
//         set => SetValue(ItemsSourceProperty
//                       , value);
//     }
//
//     public static readonly BindableProperty ItemsSourceProperty
//             = BindableProperty.Create(nameof(ItemsSource)
//                                     , typeof(ObservableCollection<SoundButtonViewModel>)
//                                     , typeof(TileCanvasView)
//                                     , propertyChanged: OnItemsSourceChanged);
//
//     private TileCanvasViewModel TileCanvasVm => (TileCanvasViewModel)BindingContext;
//
//     public TileCanvasView()
//     {
//         InitializeComponent();
//     }
//
//     protected override void OnBindingContextChanged()
//     {
//         base.OnBindingContextChanged();
//
//         if (BindingContext is TileCanvasViewModel cvm)
//         {
//             cvm.Buttons.CollectionChanged -= OnButtonsChanged;
//             cvm.Buttons.CollectionChanged += OnButtonsChanged;
//             RenderTiles();
//         }
//     }
//
//     private void OnButtonsChanged (object?                          sender
//                                  , NotifyCollectionChangedEventArgs e)
//         => RenderTiles();
//
//     private static void OnItemsSourceChanged (BindableObject bindable
//                                             , object         oldValue
//                                             , object         newValue)
//     {
//         var control = (TileCanvasView)bindable;
//
//         // Unsubscribe from old INotifyCollectionChanged
//         if (oldValue is INotifyCollectionChanged oldCollection)
//             oldCollection.CollectionChanged -= control.OnCollectionChanged;
//
//         // Subscribe to new collection
//         if (newValue is INotifyCollectionChanged newCollection)
//             newCollection.CollectionChanged += control.OnCollectionChanged;
//
//         // Render immediately
//         control.RenderTiles();
//     }
//
//     private void OnCollectionChanged (object                           sender
//                                     , NotifyCollectionChangedEventArgs e)
//     {
//         RenderTiles();
//     }
//
//     void RenderTiles()
//     {
//         var vm = TileCanvasVm; // always the one you bound in MainPage
//         Console.WriteLine($"[CanvasVM] RenderTiles on: {vm.GetHashCode()}, count={vm.Buttons.Count}");
//
//         CanvasLayout.Children.Clear(); // canvasLayout is your AbsoluteLayout x:Name
//
//         foreach (var tileVm in vm.Buttons)
//         {
//             var tileView = new TileButtonView
//                            {
//                                    BindingContext = tileVm
//                            };
//             AbsoluteLayout.SetLayoutBounds(tileView
//                                          , new Rect(tileVm.X
//                                                   , tileVm.Y
//                                                   , 100
//                                                   , 100));
//             AbsoluteLayout.SetLayoutFlags(tileView
//                                         , AbsoluteLayoutFlags.None);
//
//             CanvasLayout.Children.Add(tileView);
//         }
//     }
// }
