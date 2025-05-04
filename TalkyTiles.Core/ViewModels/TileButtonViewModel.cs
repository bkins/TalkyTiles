using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TalkyTiles.Core.Models;
using TalkyTiles.Core.Services;

namespace TalkyTiles.Core.ViewModels;

public partial class TileButtonViewModel : ObservableObject
{
    private readonly AudioService _audioService;
    private const    double       Tolerance = 0.01;

    [ObservableProperty] private TileButtonModel _model;

    public double X
    {
        get => Model.X;
        set
        {
            if (Math.Abs(Model.X - value) < Tolerance) return;

            Model.X = value;
            OnPropertyChanged(nameof(X));
        }
    }

    public double Y
    {
        get => Model.Y;
        set
        {
            if (Math.Abs(Model.Y - value) < Tolerance) return;

            Model.Y = value;
            OnPropertyChanged(nameof(Y));
        }
    }

    public TileButtonViewModel (TileButtonModel model
                              , AudioService    audioService)
    {
        _model        = model;
        _audioService = audioService;
    }

    [RelayCommand]
    private void PlayAudio()
    {
        if (string.IsNullOrEmpty(Model.AudioPath)) return;
        try
        {
            _audioService.Play(Model.AudioPath);
        }
        catch (Exception ex)
        {
            // TODO: Handle/log error
            throw new NotImplementedException("Implement error handling in TileButtonViewModel.PlayAudio");
        }
    }


}
