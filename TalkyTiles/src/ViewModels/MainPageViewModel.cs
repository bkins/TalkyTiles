using System.Collections.ObjectModel;
using System.ComponentModel;
using TalkyTiles.Models;
using TalkyTiles.Services;
using TalkyTiles.Services.Interfaces;

namespace TalkyTiles.ViewModels;

public class MainPageViewModel : INotifyPropertyChanged
{
    public ObservableCollection<SoundButtonViewModel> Buttons     { get; } = new();

    public SoundPage? CurrentPage { get; private set; }

    private readonly StorageService _storageService;
    private readonly IAudioService  _audioService;

    public MainPageViewModel (IAudioService  audioService
                            , StorageService storageService)
    {
        _storageService = storageService;
        _audioService   = audioService;

        //_ = SeedTestData();
        LoadButtons();
    }


    private async void LoadButtons()
    {
        var pages     = await _storageService.LoadAllPagesAsync();
        var firstPage = pages.FirstOrDefault();

        if (firstPage == null) return;

        CurrentPage = firstPage;

        Buttons.Clear();
        foreach (var button in firstPage.Buttons)
        {
            Buttons.Add(new SoundButtonViewModel(button
                                               , _audioService));
        }
    }

    public async Task InitializeAsync()
    {
        var pages     = await _storageService.LoadAllPagesAsync();
        var firstPage = pages.FirstOrDefault();

        if (firstPage is null) return;

        CurrentPage = firstPage;

        Buttons.Clear();
        foreach (var button in firstPage.Buttons)
        {
            Buttons.Add(new SoundButtonViewModel(button
                                               , _audioService));
        }
    }

    public async Task SeedTestData()
    {
        var samplePage = new SoundPage
                         {
                                 Name = "Test Page"
                               , Buttons =
                                 [
                                         new SoundButton
                                         {
                                                 Text      = "Hello"
                                               , AudioPath = "path/to/sound.wav"
                                         }
                                       , new SoundButton
                                         {
                                                 Text      = "Goodbye"
                                               , AudioPath = "path/to/bye.wav"
                                         }
                                 ]
                         };

        await _storageService.SavePageAsync(samplePage);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public async Task SavePageAsync()
    {
        if (CurrentPage != null)
            await _storageService.SavePageAsync(CurrentPage);
    }
}
