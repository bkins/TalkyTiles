using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TalkyTiles.Core.Models;
using TalkyTiles.Core.Services.Interfaces;
using TalkyTiles.Core.Utilities.Extensions;

// needed for TextToSpeech

namespace TalkyTiles.Core.ViewModels;

public partial class SoundButtonViewModel : ObservableObject
{
    private readonly SoundButton         _model;
    private readonly IAudioService       _audioService;
    private readonly ITileStorageService _storage;
    private readonly IUiStateService     _uiState;

    private string? _pendingAudioPath;

    public string AudioPath
    {
        get => _model.AudioPath;
        set
        {
            if (_model.AudioPath == value) return;

            _model.AudioPath = value;
            OnPropertyChanged();
        }
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanRecord))]
    private bool _isRecording;

    //public bool IsRecording
    // {
    //     get => _isRecording;
    //     set => SetProperty(ref _isRecording
    //                      , value);
    // }

    public bool CanRecord => ! _isRecording && _uiState.IsEditMode;

    private int _secondsRemaining;

    public int SecondsRemaining
    {
        get => _secondsRemaining;
        set => SetProperty(ref _secondsRemaining
                         , value);
    }

    public string DisplayText
    {
        get => _model.Text;
        set => _model.Text = value;
    }

    public string Id
    {
        get => _model.Id;
        set => _model.Id = value;
    }
    public double X
    {
        get => _model.X;
        set
        {
            if (Math.Abs(_model.X - value) < 0.01) return;
            _model.X = value;
            OnPropertyChanged();
        }
    }

    public double Y
    {
        get => _model.Y;
        set
        {
            if (Math.Abs(_model.Y - value) < 0.01) return;
            _model.Y = value;
            OnPropertyChanged();
        }
    }

    public ICommand PlayAudioCommand { get; }
    public double   Width            { get; set; }
    public double   Height           { get; set; }

    /// <summary>
    /// Mirrors the app’s global “Edit Mode” flag, so each tile can show/hide its delete button.
    /// </summary>
    public bool IsEditMode => _uiState.IsEditMode;
    public string Color { get; set; }

    public IAsyncRelayCommand RemoveCommand { get; }
    public IAsyncRelayCommand EditCommand   { get; }

    public SoundButtonViewModel (SoundButton         model
                               , IAudioService       audioService
                               , ITileStorageService storage
                               , IUiStateService     uiState
                               , TileCanvasViewModel canvasVm
                               , IAsyncRelayCommand  removeCommand)
    {
        _model        = model;
        _audioService = audioService;
        _storage      = storage;
        _uiState      = uiState;

        PlayAudioCommand = new AsyncRelayCommand(PlayAudioAsync);

        _uiState.EditModeChanged += (_, _) => OnPropertyChanged(nameof(IsEditMode));
        _uiState.EditModeChanged += (_, _) => OnPropertyChanged(nameof(CanRecord));

        RemoveCommand = removeCommand;

        //RemoveCommand = new AsyncRelayCommand(async () => await canvasVm.RemoveTileAsync(this));
        EditCommand = new AsyncRelayCommand(async () => await canvasVm.EditTileAsync(this));
    }

    private async Task PlayAudioAsync()
    {
        //TODO: Implement when saving audio is an option
        if (_model.AudioPath.HasValue())
        {
            try
            {
                await _audioService.PlayAudioAsync(_model.AudioPath);

                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        // Fallback to Text-to-Speech
        if (_model.Text.HasValue())
        {
            try
            {
                await TextToSpeech.Default.SpeakAsync(_model.Text);
            }
            catch (Exception ex)
            {
                // TODO: log TTS error
            }
        }
    }

    [RelayCommand]
    private async Task RecordAudioAsync()
    {
        var fileName = $"{_model.Id}.wav";

        try
        {
            var path = await _audioService.StartRecordingAsync(fileName);
            if (path.HasValueNullable())
            {
                _pendingAudioPath = path;
                SecondsRemaining  = 10;
                IsRecording       = true;

                _ = StartCountdownAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Recording failed: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task StopRecordingAsync()
    {
        await _audioService.StopRecordingAsync();

        if (_pendingAudioPath.HasValueNullable())
        {
            _model.AudioPath  = _pendingAudioPath ?? string.Empty;
            _pendingAudioPath = null;

            var mainPageViewModel = new MainPageViewModel(_audioService
                                                        , _storage
                                                        , _uiState
                                                        , new TileCanvasViewModel(_audioService
                                                                                , _storage
                                                                                , _uiState));

            await mainPageViewModel.SavePageAsync();

            OnPropertyChanged(nameof(AudioPath));

            await _audioService.PlayAudioAsync(_model.AudioPath);
        }

        IsRecording = false;
    }

    [RelayCommand]
    public async Task RemoveMeAsync()
    {
        // remove itself from the canvas VM
        //await _uiState.Canvas.RemoveTileAsync(this);
    }


    private async Task StartCountdownAsync()
    {
        while (SecondsRemaining > 0)
        {
            await Delay(1000);
            SecondsRemaining--;

            if (IsRecording.Not())
                return;
        }

        await StopRecordingAsync();
    }

    protected virtual Task Delay (int milliseconds)
    {
        return Task.Delay(milliseconds);
    }

    public SoundButton ToModel() => new()
                                    {
                                            Id        = _model.Id
                                          , Text      = _model.Text
                                          , AudioPath = _model.AudioPath
                                          , X         = X
                                          , Y         = Y
                                          , Width     = _model.Width
                                          , Height    = _model.Height
                                    };

}
