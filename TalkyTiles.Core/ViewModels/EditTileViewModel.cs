using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TalkyTiles.Core.ViewModels;
using TalkyTiles.Core.Models;
using System.Windows.Input;

namespace TalkyTiles.Core.ViewModels;

public partial class EditTileViewModel : ObservableObject
{
    private readonly SoundButtonViewModel _original;

    [ObservableProperty] private string _label;
    [ObservableProperty] private string _selectedColor;

    public ObservableCollection<string> AvailableColors { get; } = new()
                                                                   {
                                                                           "LightCoral", "LightBlue", "LightGreen", "LightYellow", "Plum"
                                                                   };

    public ICommand SaveCommand   { get; }
    public ICommand CancelCommand { get; }

    public event Action? CloseRequested;

    public EditTileViewModel (SoundButtonViewModel original)
    {
        _original = original;

        Label         = _original.DisplayText;
        SelectedColor = _original.Color;

        SaveCommand   = new RelayCommand(OnSave);
        CancelCommand = new RelayCommand(OnCancel);
    }

    private void OnSave()
    {
        _original.DisplayText = Label;
        _original.Color       = SelectedColor;
        CloseRequested?.Invoke();
    }

    private void OnCancel()
    {
        CloseRequested?.Invoke();
    }
}
