using TalkyTiles.Core.ViewModels;
using TalkyTiles.MobileApp.Services;

namespace TalkyTiles.MobileApp.Views;

public partial class TileButtonView : ContentView
{
    private double _x;
    private double _y;

    private double _dragStartX;
    private double _dragStartY;

    public TileButtonView()
    {
        InitializeComponent();

        this.BindingContextChanged += (s, e) =>
        {
            Content.BindingContext = BindingContext;
        };
    }

    public TileButtonView (TileButtonViewModel viewModel) : this()
    {
        //BindingContext = viewModel;
    }

    async void OnPanUpdated (object              sender
                           , PanUpdatedEventArgs e)
    {
        if (Application.Current?.MainPage is NavigationPage
            {
                    CurrentPage: MainPage
                    {
                            BindingContext: MainPageViewModel
                            {
                                    IsEditMode: false
                            }
                    }
            })
        {
            return;
        }

        if (BindingContext is not SoundButtonViewModel vm) return;

        switch (e.StatusType)
        {
            case GestureStatus.Started:
                break;

            case GestureStatus.Running:
            {
                // Move the tile visually
                var bounds = AbsoluteLayout.GetLayoutBounds(this);
                var newX   = bounds.X + e.TotalX;
                var newY   = bounds.Y + e.TotalY;

                AbsoluteLayout.SetLayoutBounds(this
                                             , new Rect(newX
                                                      , newY
                                                      , bounds.Width
                                                      , bounds.Height));

                break;
            }

            case GestureStatus.Completed:
            {
                // Save the final position to the ViewModel
                var bounds = AbsoluteLayout.GetLayoutBounds(this);

                vm.X = bounds.X;
                vm.Y = bounds.Y;

                _ = SavePosition(vm); // fire and forget

                break;
            }
        }
    }


    private static async Task SavePosition (SoundButtonViewModel vm)
    {
        Console.WriteLine($"App.Current.MainPage: {Application.Current?.MainPage}");
        Console.WriteLine($"Type: {Application.Current?.MainPage?.GetType().Name}");

        if (Application.Current?.MainPage is NavigationPage
            {
                    CurrentPage: MainPage
                    {
                            BindingContext: MainPageViewModel
                            {
                                    CurrentPage: not null
                            } mainVM
                    }
            })
        {
            await mainVM.SavePageAsync();
        }
        else
        {
            Console.WriteLine("SavePosition context NOT matched — something is null or mismatched.");
        }

        // if (Application.Current
        //                ?.MainPage is MainPage
        //     {
        //             BindingContext: MainPageViewModel
        //             {
        //                     CurrentPage: { } page
        //             } mainPageViewModel
        //     })
        // {
        //     Console.WriteLine($"Saving position for: {vm.DisplayText} => X: {vm.X}, Y: {vm.Y}");
        //     await mainPageViewModel.SavePageAsync();
        // }
    }
}
