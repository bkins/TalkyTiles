namespace TalkyTiles.Core.Models;

public class SoundPage
{
    public string            Id      { get; set; } = Guid.NewGuid().ToString();
    public string            Name    { get; set; }
    public List<SoundButton> Buttons { get; set; } = new();
}
