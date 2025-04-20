namespace TalkyTiles.Models;

public class SoundButton
{
    public string Id        { get; set; } = Guid.NewGuid().ToString();
    public string Text      { get; set; }
    public string ImagePath { get; set; }
    public string AudioPath { get; set; }
    public double X         { get; set; }
    public double Y         { get; set; }
    public double Width     { get; set; } = 100;
    public double Height    { get; set; } = 100;
}
