using System.Windows.Input;
using TalkyTiles.Core.ViewModels;

namespace TalkyTiles.MobileApp.Views;

public partial class TileButtonView : ContentView
{
    public static readonly BindableProperty RemoveTileCommandProperty =
            BindableProperty.Create(nameof(RemoveTileCommand)
                                  , typeof(ICommand)
                                  , typeof(TileButtonView));

    public static readonly BindableProperty EditTileCommandProperty =
            BindableProperty.Create(nameof(EditTileCommand)
                                  , typeof(ICommand)
                                  , typeof(TileButtonView));

    public ICommand? RemoveTileCommand
    {
        get => (ICommand?)GetValue(RemoveTileCommandProperty);
        set => SetValue(RemoveTileCommandProperty
                      , value);
    }

    public ICommand? EditTileCommand
    {
        get => (ICommand?)GetValue(EditTileCommandProperty);
        set => SetValue(EditTileCommandProperty
                      , value);
    }

    public TileButtonView()
    {
        InitializeComponent();

        BindingContextChanged += (_, _) =>
        {
            Content.BindingContext = BindingContext;
        };
    }
}


// using System.Windows.Input;
// using TalkyTiles.Core.Utilities.Extensions;
// using TalkyTiles.Core.ViewModels;
// using TalkyTiles.MobileApp.Services;
// using TalkyTiles.MobileApp.UI;
//
// namespace TalkyTiles.MobileApp.Views;
//
// public partial class TileButtonView : ContentView
// {
//     private double _x;
//     private double _y;
//
//     private double _dragStartX;
//     private double _dragStartY;
//     private double _lastX;
//     private double _lastY;
//
//     private double _canvasWidth;
//     double         _canvasHeight;
//
//     double         _startX, _startY, _tileW, _tileH;
//
//     public TileButtonView()
//     {
//         InitializeComponent();
//
//         BindingContextChanged += (s, e) =>
//         {
//             Content.BindingContext = BindingContext;
//         };
//     }
//
//     public static readonly BindableProperty RemoveTileCommandProperty = BindableProperty.Create(nameof(RemoveTileCommand)
//                                                                                               , typeof(ICommand)
//                                                                                               , typeof(TileButtonView));
//
//     public ICommand? RemoveTileCommand
//     {
//         get => (ICommand?)GetValue(RemoveTileCommandProperty);
//         set => SetValue(RemoveTileCommandProperty
//                       , value);
//     }
//
//     public TileButtonView (TileButtonViewModel viewModel) : this()
//     {
//         //BindingContext = viewModel;
//     }
//
//     async void OnPanUpdated(object sender, PanUpdatedEventArgs e)
//     {
//         // 1) Early exits
//         if (BindingContext is not SoundButtonViewModel vm) return;
//         if (Parent is not AbsoluteLayout layout) return;
//
//         switch (e.StatusType)
//         {
//             case GestureStatus.Started:
//             {
//                 // Grab the *actual* bounds of this tile
//                 var b = AbsoluteLayout.GetLayoutBounds(this);
//                 _startX = b.X;
//                 _startY = b.Y;
//                 _tileW   = b.Width;
//                 _tileH   = b.Height;
//                 break;
//             }
//
//             case GestureStatus.Running:
//             {
//                 // Only translate the view visually — no LayoutBounds yet
//                 TranslationX = e.TotalX;
//                 TranslationY = e.TotalY;
//                 break;
//             }
//
//             case GestureStatus.Completed:
//             {
//                 // Compute raw new origin
//                 var rawX = _startX + e.TotalX;
//                 var rawY = _startY + e.TotalY;
//
//                 // Clamp into the canvas’s measured size
//                 var maxX = Math.Max(0, layout.Width  - _tileW);
//                 var maxY = Math.Max(0, layout.Height - _tileH);
//
//                 var finalX = Math.Clamp(rawX, 0, maxX);
//                 var finalY = Math.Clamp(rawY, 0, maxY);
//
//                 // 1) Update your ViewModel
//                 vm.X = finalX;
//                 vm.Y = finalY;
//
//                 // 2) Commit into the AbsoluteLayout once
//                 AbsoluteLayout.SetLayoutBounds(
//                     this,
//                     new Rect(finalX, finalY, _tileW, _tileH)
//                 );
//
//                 // 3) Zero out any translation so it sits flush
//                 TranslationX = 0;
//                 TranslationY = 0;
//
//                 // 4) Fire-and-forget the save so UI isn’t blocked
//                 _ = SavePosition();
//                 break;
//             }
//
//             case GestureStatus.Canceled:
//             {
//                 // If you want the tile to snap back on cancel:
//                 TranslationX = 0;
//                 TranslationY = 0;
//                 break;
//             }
//         }
//     }
//
//
//
//
//
//     // async void OnPanUpdated (object              sender
//     //                        , PanUpdatedEventArgs e)
//     // {
//     //     if (Application.Current?.MainPage is NavigationPage
//     //         {
//     //                 CurrentPage: MainPage
//     //                 {
//     //                         BindingContext: MainPageViewModel
//     //                         {
//     //                                 IsEditMode: false
//     //                         }
//     //                 }
//     //         })
//     //     {
//     //         return;
//     //     }
//     //
//     //     if (BindingContext is not SoundButtonViewModel vm) return;
//     //
//     //     switch (e.StatusType)
//     //     {
//     //         case GestureStatus.Started:
//     //
//     //             if (Parent is AbsoluteLayout thisLayout)
//     //             {
//     //                 // record the *real* canvas dimensions at the start of drag
//     //                 _canvasWidth  = thisLayout.Width;
//     //                 _canvasHeight = thisLayout.Height;
//     //             }
//     //
//     //             _dragStartX = vm.X;
//     //             _dragStartY = vm.Y;
//     //
//     //             break;
//     //
//     //         case GestureStatus.Running:
//     //         {
//     //             if (Parent is not AbsoluteLayout) break;
//     //
//     //             // compute raw positions
//     //             var rawX = _dragStartX + e.TotalX;
//     //             var rawY = _dragStartY + e.TotalY;
//     //
//     //             // clamp within [0 .. canvasSize – tileSize]
//     //             // assume your tiles are 100×100; if variable, read vm.Width/vm.Height or view.Width/Height
//     //             var maxX = _canvasWidth - 100;
//     //             var maxY = _canvasHeight - 100;
//     //             var newX = Math.Clamp(rawX
//     //                                 , 0
//     //                                 , maxX);
//     //             var newY = Math.Clamp(rawY
//     //                                 , 0
//     //                                 , maxY);
//     //
//     //             // update VM *and* view in lock-step
//     //             vm.X = newX;
//     //             vm.Y = newY;
//     //             AbsoluteLayout.SetLayoutBounds(this
//     //                                          , new Rect(newX
//     //                                                   , newY
//     //                                                   , 100
//     //                                                   , 100));
//     //             // // Move the tile visually
//     //             // var bounds = AbsoluteLayout.GetLayoutBounds(this);
//     //             // var newX   = bounds.X + e.TotalX;
//     //             // var newY   = bounds.Y + e.TotalY;
//     //             //
//     //             // AbsoluteLayout.SetLayoutBounds(this
//     //             //                              , new Rect(newX
//     //             //                                       , newY
//     //             //                                       , bounds.Width
//     //             //                                       , bounds.Height));
//     //
//     //             break;
//     //         }
//     //
//     //         case GestureStatus.Completed:
//     //         {
//     //             _ = SavePosition();
//     //
//     //             // // Save the final position to the ViewModel
//     //             // var bounds = AbsoluteLayout.GetLayoutBounds(this);
//     //             //
//     //             // vm.X = bounds.X;
//     //             // vm.Y = bounds.Y;
//     //             //
//     //             // _ = SavePosition(); // fire and forget
//     //
//     //             break;
//     //         }
//     //     }
//     // }
//
//
//     private static async Task SavePosition ()
//     {
//         Console.WriteLine($"App.Current.MainPage: {Application.Current?.MainPage}");
//         Console.WriteLine($"Type: {Application.Current?.MainPage?.GetType().Name}");
//
//         if (Application.Current?.MainPage is NavigationPage
//             {
//                     CurrentPage: MainPage
//                     {
//                             BindingContext: MainPageViewModel
//                             {
//
//                             } mainVm
//                     }
//             })
//         {
//             await mainVm.SavePageAsync();
//         }
//         else
//         {
//             Console.WriteLine("SavePosition context NOT matched — something is null or mismatched.");
//         }
//
//         // if (Application.Current
//         //                ?.MainPage is MainPage
//         //     {
//         //             BindingContext: MainPageViewModel
//         //             {
//         //                     CurrentPage: { } page
//         //             } mainPageViewModel
//         //     })
//         // {
//         //     Console.WriteLine($"Saving position for: {vm.DisplayText} => X: {vm.X}, Y: {vm.Y}");
//         //     await mainPageViewModel.SavePageAsync();
//         // }
//     }
//
//     private async void OnEditClicked (object?   sender
//                                     , EventArgs e)
//     {
//         if (BindingContext is not SoundButtonViewModel vm)
//             return;
//
//         var editVm   = new EditTileViewModel(vm);
//         var editPage = new EditTilePage { BindingContext = editVm };
//
//         // Use async lambda to handle dismissal
//         editVm.CloseRequested += async () =>
//         {
//             await MainThread.InvokeOnMainThreadAsync(() => Application.Current!
//                                                                       .MainPage!
//                                                                       .Navigation
//                                                                       .PopModalAsync());
//         };
//
//         await MainThread.InvokeOnMainThreadAsync(() => Application.Current!
//                                                                   .MainPage!
//                                                                   .Navigation
//                                                                   .PushModalAsync(editPage));
//     }
//
// }
