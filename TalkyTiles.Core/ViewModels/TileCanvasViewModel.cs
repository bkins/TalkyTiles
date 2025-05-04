using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TalkyTiles.Core.Models;
using TalkyTiles.Core.Services.Interfaces;
using TalkyTiles.Core.Utilities.Extensions;


namespace TalkyTiles.Core.ViewModels;

/// <summary>
/// ViewModel to manage a canvas of tiles: loading, adding, saving positions, etc.
/// </summary>
public partial class TileCanvasViewModel : ObservableObject
{
    private StringBuilder traceLog = new StringBuilder();

    // Collection of individual tile VMs
    public ObservableCollection<SoundButtonViewModel> Buttons { get; } = new();

    private          SoundPage?          _currentPage;
    private readonly IAudioService       _audio;
    private readonly ITileStorageService _storage;
    private readonly IUiStateService     _uiState;

    const int    Columns  = 3;
    const int    Rows     = 5;
    const int    MaxTiles = Columns * Rows;
    const double TileW    = 100; // or read from a constant
    const double TileH    = 100;
    const double Spacing  = 10;

    public TileCanvasViewModel (IAudioService       audioService
                              , ITileStorageService storageService
                              , IUiStateService     uiStateService)
    {
        traceLog.AppendLine($"[CanvasVM] ctor: {GetHashCode()}");

        _audio   = audioService;
        _storage = storageService;
        _uiState = uiStateService;
    }

    /// <summary>
    /// Load a specific page by ID and populate the Buttons collection.
    /// </summary>
    public async Task LoadPageAsync (string pageId)
    {
        var page = await _storage.LoadPageAsync(pageId);

        if (page == null)
            return;

        _currentPage = page;

        Buttons.Clear();
        foreach (var btn in page.Buttons)
        {
            Buttons.Add(new SoundButtonViewModel(btn
                                               , _audio
                                               , _storage
                                               , _uiState
                                               , new TileCanvasViewModel(_audio
                                                                       , _storage
                                                                       , _uiState)
                                               , RemoveTileCommand));
        }

        traceLog.AppendLine($"[CanvasVM] LoadPageAsync on: {GetHashCode()} – loading {Buttons.Count} Buttons");

    }

    /// <summary>
    /// Adds a new tile at a default location and persists it.
    /// </summary>
    [RelayCommand]
    public async Task AddNewTileAsync()
    {
        // enforce limit
        if (Buttons.Count >= MaxTiles) return;

        // compute grid slot
        int idx = Buttons.Count;
        int col = idx % Columns;
        int row = idx / Columns;

        double x = col * (TileW + Spacing);
        double y = row * (TileH + Spacing);

        // create & register model
        var model = new SoundButton { Text = "New", X = x, Y = y };
        _currentPage.Buttons.Add(model);

        // wrap VM
        var vm = new SoundButtonViewModel(model
                                        , _audio
                                        , _storage
                                        , _uiState
                                        , new TileCanvasViewModel(_audio
                                                                , _storage
                                                                , _uiState)
                                        , RemoveTileCommand);
        Buttons.Add(vm);

        // persist
        await _storage.SavePageAsync(_currentPage);
    }
    // [RelayCommand]
    // public async Task AddNewTileAsync()
    // {
    //     if (_currentPage == null)
    //         return;
    //
    //     var newTile = new SoundButton { Text = "New", X = 50, Y = 50 };
    //     _currentPage.Buttons.Add(newTile);
    //     Buttons.Add(new SoundButtonViewModel(newTile
    //                                        , _audio
    //                                        , _storage
    //                                        , _uiState));
    //     await _storage.SavePageAsync(_currentPage);
    // }

    /// <summary>
    /// Saves the current positions of all tiles back to storage.
    /// </summary>
    [RelayCommand]
    public async Task SaveCanvasAsync()
    {
        if (_currentPage == null)
            return;

        // Sync positions back to model
        _currentPage.Buttons = Buttons.Select(buttonViewModel => buttonViewModel.ToModel()).ToList();
        await _storage.SavePageAsync(_currentPage);
    }

    [RelayCommand]
    public async Task RemoveTileAsync (SoundButtonViewModel soundButtonVm)
    {
        traceLog.AppendLine($"[CanvasVM] RemoveTileAsync on: {GetHashCode()} – Buttons.Count={Buttons.Count}");

        if (Buttons.Contains(soundButtonVm).Not()) return;

        Buttons.Remove(soundButtonVm);
        if (_currentPage is not null)
        {
            _currentPage.Buttons.Remove(soundButtonVm.ToModel());
            await _storage.SavePageAsync(_currentPage);
        }

    }

    /// <summary>
    /// Called when the user taps the edit-button on a tile.
    /// </summary>
    [RelayCommand]
    public async Task EditTileAsync (SoundButtonViewModel soundButtonVm)
    {
        await Shell.Current.GoToAsync($"EditTilePage?tileId={soundButtonVm.Id}");
    }

    [RelayCommand]
    public async Task ClearTilesAsync()
    {
        Buttons.Clear();
        _currentPage?.Buttons.Clear();

        if (_currentPage != null) await _storage.SavePageAsync(_currentPage);
    }

    public async Task InitializeAsync()
    {
        // 1) Load _all_ pages
        var pages = await _storage.LoadAllPagesAsync();

        // 2) If none exist, create & save a brand-new one
        if (pages.Count == 0)
        {
            var seed = new SoundPage { Name = "Page 1" };
            // you can seed default buttons here if you like
            await _storage.SavePageAsync(seed);
            pages = new List<SoundPage> { seed };
        }

        // 3) Take the first page and populate the canvas
        var page = pages.First();
        await LoadPageAsync(page.Id);
    }

}
