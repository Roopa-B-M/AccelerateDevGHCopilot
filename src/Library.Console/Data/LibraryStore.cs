using System.Text.Json;
using Library.Console.Models;

namespace Library.Console.Data;

public class LibraryStore
{
    private readonly string _dbPath;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    public LibraryStore()
    {
        _dbPath = Path.Combine(AppContext.BaseDirectory, "library.json");
    }

    public List<Book> Load()
    {
        if (!File.Exists(_dbPath)) return new List<Book>();
        var json = File.ReadAllText(_dbPath);
        return JsonSerializer.Deserialize<List<Book>>(json, _jsonOptions) ?? new List<Book>();
    }

    public void Save(List<Book> books)
    {
        var json = JsonSerializer.Serialize(books, _jsonOptions);
        File.WriteAllText(_dbPath, json);
    }
}
