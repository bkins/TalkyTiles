using TalkyTiles.Core.Services.Interfaces;
using TalkyTiles.Core.Utilities.Extensions;

namespace TalkyTiles.Core.Services;

public class UiStateService : IUiStateService
{
    private bool _isEditMode;

    public bool IsEditMode
    {
        get => _isEditMode;
        private set
        {
            if (_isEditMode == value) return;

            _isEditMode = value;
            EditModeChanged?.Invoke(this
                                  , EventArgs.Empty);
        }
    }

    public event EventHandler? EditModeChanged;

    public void ToggleEditMode()
    {
        IsEditMode = IsEditMode.Not();
    }
}
