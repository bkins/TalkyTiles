using TalkyTiles.Models;
using TalkyTiles.Services;
using TalkyTiles.ViewModels;

namespace TalkyTiles.Views;

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
        if (BindingContext is not SoundButtonViewModel vm) return;

        switch (e.StatusType)
        {
            case GestureStatus.Started:

                _dragStartX = vm.X;
                _dragStartY = vm.Y;

                break;

            case GestureStatus.Running:

                var newX = _dragStartX + e.TotalX;
                var newY = _dragStartY + e.TotalY;

                // Update position visually
                if (Parent is AbsoluteLayout layout)
                {
                    AbsoluteLayout.SetLayoutBounds(this
                                                 , new Rect(newX
                                                          , newY
                                                          , 100
                                                          , 100));
                }

                break;

            case GestureStatus.Completed:
            {
                var finalX = _dragStartX + e.TotalX;
                var finalY = _dragStartY + e.TotalY;

                vm.X = finalX;
                vm.Y = finalY;

                var distance = Math.Sqrt(Math.Pow(vm.X - _x, 2)
                                       + Math.Pow(vm.Y - _y, 2));
                if (distance < 5)
                {
                    // TODO: Decide: Treat as a tap? or ignore drag
                }
                else
                {
                    await SavePosition();
                }

                break;
            }
            case GestureStatus.Canceled:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static async Task SavePosition()
    {

        if (Application.Current
                       ?.MainPage is MainPage
            {
                    BindingContext: MainPageViewModel
                    {
                            CurrentPage: { } page
                    } mainPageViewModel
            })
        {
            await mainPageViewModel.SavePageAsync();
        }
    }
}
