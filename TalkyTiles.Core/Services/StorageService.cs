using System.Text.Json;
using TalkyTiles.Core.Models;

namespace TalkyTiles.Core.Services;

public class StorageService
{
    private readonly string _pagesFolder;

    public StorageService()
    {
        var root = FileSystem.AppDataDirectory;
        _pagesFolder = Path.Combine(root
                                 , "pages");
        Directory.CreateDirectory(_pagesFolder);
    }

    public async Task SavePageAsync (SoundPage page)
    {
        string filePath = Path.Combine(_pagesFolder
                                     , $"{page.Id}.json");
        string json = JsonSerializer.Serialize(page
                                             , new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath
                                   , json);
    }

    public async Task<SoundPage?> LoadPageAsync (string id)
    {
        string filePath = Path.Combine(_pagesFolder
                                     , $"{id}.json");

        if (! File.Exists(filePath)) return null;
        string json = await File.ReadAllTextAsync(filePath);

        return JsonSerializer.Deserialize<SoundPage>(json);
    }

    public async Task<List<SoundPage>> LoadAllPagesAsync()
    {
        var pages = new List<SoundPage>();
        var jsonFiles = Directory.GetFiles(_pagesFolder
                                         , "*.json");
        try
        {
            foreach (var file in jsonFiles)
            {
                var json = await File.ReadAllTextAsync(file);
                var page = JsonSerializer.Deserialize<SoundPage>(json);
                if (page != null) pages.Add(page);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

        }

        return pages;
    }

    public void DeletePage (string id)
    {
        string filePath = Path.Combine(_pagesFolder
                                     , $"{id}.json");
        if (File.Exists(filePath)) File.Delete(filePath);
    }
}
