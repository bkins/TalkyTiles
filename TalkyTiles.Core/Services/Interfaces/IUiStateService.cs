namespace TalkyTiles.Core.Services.Interfaces;

public interface IUiStateService
{
    bool                IsEditMode { get; }
    void                ToggleEditMode();
    event EventHandler? EditModeChanged;
}
