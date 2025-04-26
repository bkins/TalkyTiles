using TalkyTiles.Core.Services.Interfaces;
using Plugin.Maui.Audio;
using TalkyTiles.Core.Utilities.Extensions;

namespace TalkyTiles.Core.Services;

public class AudioService : IAudioService
{
    private readonly IAudioManager   _audioManager;
    private          IAudioRecorder? _recorder;
    private          string          _path;

    public AudioService()
    {
        _audioManager = AudioManager.Current;
    }

    public void Play (string audioPath)
    {
        if (File.Exists(audioPath).Not()) return;

        var player = _audioManager.CreatePlayer(File.OpenRead(audioPath));
        player.Play();
    }

    public async Task<string?> StartRecordingAsync (string fileName)
    {
        Console.WriteLine($"Recording started at: {_path}");

        // Ensure permissions
        var micStatus = await Permissions.RequestAsync<Permissions.Microphone>();

        if (micStatus != PermissionStatus.Granted)
        {
            // await App.Current
            //          .MainPage
            //          .DisplayAlert("Permission Needed"
            //                      , "Microphone access is required to record audio."
            //                      , "OK");
            Console.WriteLine("Permission Needed: Microphone access is required to record audio.");
            return null;
        }

        _path = Path.Combine(FileSystem.AppDataDirectory
                              , fileName);

        _recorder = _audioManager.CreateRecorder();
        await _recorder.StartAsync(_path);

        return _path;
    }

    public async Task StopRecordingAsync()
    {
        if (_recorder is { IsRecording: true })
        {
            await _recorder.StopAsync();
        }

        Console.WriteLine("Recording stopped.");

        if (File.Exists(_path).Not()) Console.WriteLine($"{_path} does not exist.");

        var length = new FileInfo(_path).Length;
        if (length < 5) Console.WriteLine($"{_path} is too small.");

        Console.WriteLine($"File saved: {_path}, size: {length} bytes");

    }

    public async Task PlayAudioAsync (string path)
    {
        if (File.Exists(path).Not()) return;

        await using var stream = File.OpenRead(path);

        var player = _audioManager.CreatePlayer(stream);

        player.Play();
    }

}
