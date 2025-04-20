namespace TalkyTiles.Models;

public class TileButtonModel
{
    public string  Id          { get; set; } = Guid.NewGuid().ToString();
    public string  DisplayText { get; set; } = string.Empty;
    public string? ImagePath   { get; set; }
    public string? AudioPath   { get; set; }
    public double  X           { get; set; }
    public double  Y           { get; set; }
}
