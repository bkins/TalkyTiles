using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TalkyTiles.Core.Models;
using TalkyTiles.Core.Services.Interfaces;

namespace TalkyTiles.Core.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
    public ObservableCollection<SoundButtonViewModel> Buttons     { get; } = new();
    //public SoundPage?                                 CurrentPage { get; private set; }

    private readonly ITileStorageService _storage;
    private readonly IAudioService       _audio;
    private readonly IUiStateService     _uiState;
    private readonly StringBuilder       _log = new();

    public bool IsEditMode
    {
        get => _uiState.IsEditMode;
        set
        {
            // only toggle if the UI really flipped the value
            if (value == _uiState.IsEditMode)
                return;

            _uiState.ToggleEditMode();
            OnPropertyChanged(nameof(IsEditMode));
        }
    }

    public TileCanvasViewModel Canvas { get; }

    public MainPageViewModel (IAudioService       audio
                            , ITileStorageService storage
                            , IUiStateService     uiState
            , TileCanvasViewModel                 canvasVm)
    {
        _audio   = audio;
        _storage = storage;
        _uiState = uiState;

        Canvas   = canvasVm;

        // React to UI state changes
        _uiState.EditModeChanged += (_, _) => OnPropertyChanged(nameof(IsEditMode));
    }

    public async Task InitializeAsync()
    {
        await Canvas.InitializeAsync();

        // CurrentPage = await LoadOrSeedPageAsync();
        // LoadButtonsFromPage();
    }

    private async Task<SoundPage> LoadOrSeedPageAsync()
    {
        var pages = await _storage.LoadAllPagesAsync();
        return pages.FirstOrDefault() ?? await SeedTestDataAsync();
    }

    // private void LoadButtonsFromPage()
    // {
    //     // No-op if there's nothing to show
    //     if (CurrentPage?.Buttons is not { Count: > 0 })
    //         return;
    //
    //     Buttons.Clear();
    //     _log.AppendLine("Buttons on Load:");
    //
    //     foreach (var button in CurrentPage.Buttons)
    //     {
    //         Buttons.Add(new SoundButtonViewModel(button, _audio, _storage, _uiState));
    //         _log.AppendLine($"  {button.Text}: X={button.X}, Y={button.Y}");
    //     }
    // }

    private async Task<SoundPage> SeedTestDataAsync()
    {
        var sample = new SoundPage { Name         = "Page 1" };
        sample.Buttons.Add(new SoundButton { Text = "Hello" });
        sample.Buttons.Add(new SoundButton { Text = "Goodbye" });

        await _storage.SavePageAsync(sample);
        return sample;
    }


    [RelayCommand]
    public void ToggleEditMode()
    {
        _uiState.ToggleEditMode();
    }

    [RelayCommand]
    public Task AddNewTileAsync()
        => Canvas.AddNewTileAsync();

    [RelayCommand]
    public Task SavePageAsync()
        => Canvas.SaveCanvasAsync();

}
