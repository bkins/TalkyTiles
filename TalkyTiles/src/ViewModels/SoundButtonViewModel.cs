using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TalkyTiles.Models;
using TalkyTiles.Services.Interfaces;
using TalkyTiles.Utilities;
using Microsoft.Maui.ApplicationModel.Communication;
using Microsoft.Maui.ApplicationModel;
using TalkyTiles.Views; // needed for TextToSpeech

namespace TalkyTiles.ViewModels;

public partial class SoundButtonViewModel : ObservableObject
{
    private readonly SoundButton   _model;
    private readonly IAudioService _audioService;

    public string? AudioPath
    {
        get => _model.AudioPath;
        set
        {
            if (_model.AudioPath != value)
            {
                _model.AudioPath = value;
                OnPropertyChanged();
            }
        }
    }

    private bool _isRecording;

    public bool IsRecording
    {
        get => _isRecording;
        set => SetProperty(ref _isRecording
                         , value);
    }

    private int _secondsRemaining;

    public int SecondsRemaining
    {
        get => _secondsRemaining;
        set => SetProperty(ref _secondsRemaining
                         , value);
    }

    public SoundButtonViewModel (SoundButton   model
                               , IAudioService audioService)
    {
        _model           = model;
        _audioService    = audioService;
        PlayAudioCommand = new AsyncRelayCommand(PlayAudioAsync);
    }

    public string DisplayText => _model.Text;
    public double X
    {
        get => _model.X;
        set => _model.X = value;
    }

    public double Y
    {
        get => _model.Y;
        set => _model.Y = value;
    }

    public ICommand PlayAudioCommand { get; }

    private async Task PlayAudioAsync()
    {
        //TODO: Implement when saving audio is an option
        // if (_model.AudioPath.HasValue())
        // {
        //     try
        //     {
        //         _audioService.Play(_model.AudioPath);
        //
        //         return;
        //     }
        //     catch (Exception ex)
        //     {
        //         // TODO: log the error
        //     }
        // }

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

        var path = await _audioService.StartRecordingAsync(fileName);
        if (! string.IsNullOrEmpty(path))
        {
            _model.AudioPath = path;

            // Optional: Persist change
            if (Application.Current?.MainPage is MainPage mainPage
             && mainPage.BindingContext is MainPageViewModel { CurrentPage: { } page } mainPageViewModel)
            {
                await mainPageViewModel.SavePageAsync();
            }

            // Notify that DisplayText or other UI may need update
            OnPropertyChanged(nameof(AudioPath));
        }
    }

    [RelayCommand]
    private async Task StopRecordingAsync()
    {
        await _audioService.StopRecordingAsync();
        IsRecording = false;

        // Persist
        if (Application.Current?.MainPage is MainPage mainPage
         && mainPage.BindingContext is MainPageViewModel { CurrentPage: { } page } mainPageViewModel)
        {
            await mainPageViewModel.SavePageAsync();
        }
    }

    private async Task StartCountdownAsync()
    {
        while (SecondsRemaining > 0)
        {
            await Task.Delay(1000);
            SecondsRemaining--;

            if (! IsRecording)
                return;
        }

        await StopRecordingAsync();
    }
}
