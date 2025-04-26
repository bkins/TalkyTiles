namespace TalkyTiles.Core.Services.Interfaces;

public interface IAudioService
{
    Task          PlayAudioAsync (string      path);
    Task<string?> StartRecordingAsync (string fileName); // returns full path to file
    Task          StopRecordingAsync();
}
