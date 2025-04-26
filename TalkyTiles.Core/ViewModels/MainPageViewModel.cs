using System.Collections.ObjectModel;
using System.Text;
using TalkyTiles.Core.Models;
using TalkyTiles.Core.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TalkyTiles.Core.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
    public ObservableCollection<SoundButtonViewModel> Buttons { get; } = new();

    public SoundPage? CurrentPage { get; private set; }

    private readonly ITileStorageService _storage;
    private readonly IAudioService       _audio;
    private readonly IUiStateService     _uiState;
    private readonly StringBuilder       _log = new();

    public bool IsEditMode
    {
        get => _uiState.IsEditMode;
        set
        {
            // Only toggle if the user really flipped the switch
            if (value != _uiState.IsEditMode)
                _uiState.ToggleEditMode();
        }
    }

    public MainPageViewModel (IAudioService       audio
                            , ITileStorageService storage
                            , IUiStateService     uiState)
    {
        _audio   = audio;
        _storage = storage;
        _uiState = uiState;

        // Subscribe to UI state changes
        _uiState.EditModeChanged += (_
                                   , _) =>
        {
            OnPropertyChanged(nameof(IsEditMode));
        };

    }

    public async Task InitializeAsync()
    {

        // Load the first page (seed if necessary)
        var pages = await _storage.LoadAllPagesAsync();
        if (! pages.Any())
            await SeedTestDataAsync();

        var page = pages.FirstOrDefault() ?? await SeedTestDataAsync();
        CurrentPage = page;

        Buttons.Clear();
        _log.AppendLine("Buttons on Load:");
        foreach (var btn in page.Buttons)
        {
            Buttons.Add(new SoundButtonViewModel(btn
                                               , _audio
                                               , _storage
                                               , _uiState));
            _log.AppendLine($"  {btn.Text}: X={btn.X}, Y={btn.Y}");
        }
    }

    private async Task<SoundPage> SeedTestDataAsync()
    {
        var sample = new SoundPage { Name         = "Page 1" };
        sample.Buttons.Add(new SoundButton { Text = "Hello" });
        sample.Buttons.Add(new SoundButton { Text = "Goodbye" });
        await _storage.SavePageAsync(sample);

        return sample;
    }

    [RelayCommand]
    public async Task SavePageAsync()
    {
        if (CurrentPage == null) return;
        // Sync VM -> model
        CurrentPage.Buttons = Buttons.Select(vm => vm.ToModel()).ToList();
        await _storage.SavePageAsync(CurrentPage);
        _log.AppendLine("Page saved.");
        Console.WriteLine(_log.ToString());
    }

    [RelayCommand]
    public void ToggleEditMode()
    {
        _uiState.ToggleEditMode();
    }

    [RelayCommand]
    public async Task AddNewTileAsync()
    {
        if (CurrentPage == null) return;
        var btn = new SoundButton { Text = "New", X = 50, Y = 50 };
        CurrentPage.Buttons.Add(btn);
        Buttons.Add(new SoundButtonViewModel(btn
                                           , _audio
                                           , _storage
                                           , _uiState));
        await _storage.SavePageAsync(CurrentPage);
    }
}
