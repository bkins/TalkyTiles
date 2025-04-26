using TalkyTiles.Core.Models;

namespace TalkyTiles.Core.Services.Interfaces;

public interface ITileStorageService
{
    // Load all pages (if you have multiple pages)
    Task<List<SoundPage>> LoadAllPagesAsync();

    // Load a single page by its ID
    Task<SoundPage?> LoadPageAsync (string id);

    // Save (or overwrite) a page
    Task SavePageAsync (SoundPage page);

    // Delete a page
    Task DeletePageAsync (string id);
}
