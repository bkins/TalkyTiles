using Plugin.Maui.Audio;
using TalkyTiles.Services.Interfaces;

using Plugin.Maui.Audio;

namespace TalkyTiles.Services;

public class AudioService : IAudioService
{
    private readonly IAudioManager   _audioManager;
    private          IAudioRecorder? _recorder;

    public AudioService()
    {
        _audioManager = AudioManager.Current;
    }

    public void Play (string audioPath)
    {
        if (! File.Exists(audioPath)) return;

        var player = _audioManager.CreatePlayer(File.OpenRead(audioPath));
        player.Play();
    }

    public async Task<string?> StartRecordingAsync (string fileName)
    {
        // Ensure permissions
        var micStatus = await Permissions.RequestAsync<Permissions.Microphone>();

        if (micStatus != PermissionStatus.Granted)
            return null;

        var path = Path.Combine(FileSystem.AppDataDirectory
                              , fileName);

        _recorder = _audioManager.CreateRecorder();
        await _recorder.StartAsync(path);

        // Show UI to stop recording (coming up next)

        return path;
    }

    public async Task StopRecordingAsync()
    {
        if (_recorder != null && _recorder.IsRecording)
        {
            await _recorder.StopAsync();
        }
    }

    public async Task PlayAudioAsync (string path)
    {
        if (! File.Exists(path)) return;

        await using var stream = File.OpenRead(path);
        var       player = _audioManager.CreatePlayer(stream);

        player.Play();
    }

}
