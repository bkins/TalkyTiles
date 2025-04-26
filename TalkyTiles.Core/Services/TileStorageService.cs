using System.Text.Json;
using TalkyTiles.Core.Models;
using TalkyTiles.Core.Services.Interfaces;

namespace TalkyTiles.Core.Services;

public class TileStorageService : ITileStorageService
{
    private readonly string _pagesFolder;

    public TileStorageService()
    {
        var root = FileSystem.AppDataDirectory;
        _pagesFolder = Path.Combine(root
                                  , "pages");
        Directory.CreateDirectory(_pagesFolder);
    }

    public async Task<List<SoundPage>> LoadAllPagesAsync()
    {
        var pages = new List<SoundPage>();
        foreach (var file in Directory.GetFiles(_pagesFolder
                                              , "*.json"))
        {
            var json = await File.ReadAllTextAsync(file);
            var page = JsonSerializer.Deserialize<SoundPage>(json);
            if (page != null)
                pages.Add(page);
        }

        return pages;
    }

    public async Task<SoundPage?> LoadPageAsync (string id)
    {
        var filePath = Path.Combine(_pagesFolder
                                  , $"{id}.json");

        if (! File.Exists(filePath)) return null;
        var json = await File.ReadAllTextAsync(filePath);

        return JsonSerializer.Deserialize<SoundPage>(json);
    }

    public async Task SavePageAsync (SoundPage page)
    {
        var filePath = Path.Combine(_pagesFolder
                                  , $"{page.Id}.json");
        var json = JsonSerializer.Serialize(page
                                          , new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath
                                   , json);
    }

    public Task DeletePageAsync (string id)
    {
        var filePath = Path.Combine(_pagesFolder
                                  , $"{id}.json");
        if (File.Exists(filePath))
            File.Delete(filePath);

        return Task.CompletedTask;
    }
}
