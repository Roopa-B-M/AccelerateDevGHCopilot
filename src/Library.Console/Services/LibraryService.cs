using Library.Console.Data;
using Library.Console.Models;

namespace Library.Console.Services;

public class LibraryService
{
    private readonly LibraryStore _store = new();

    public Book? AddBook(string title, string author, string isbn)
    {
        var existing = _store.Load().FirstOrDefault(b => b.ISBN.Equals(isbn, StringComparison.OrdinalIgnoreCase));
        if (existing is not null) return null;

        var book = new Book(Guid.NewGuid(), title, author, isbn);
        var list = _store.Load();
        list.Add(book);
        _store.Save(list);
        return book;
    }

    public List<Book> ListBooks() => _store.Load();

    public bool RemoveBook(string isbn)
    {
        var list = _store.Load();
        var removed = list.RemoveAll(b => b.ISBN.Equals(isbn, StringComparison.OrdinalIgnoreCase)) > 0;
        if (removed) _store.Save(list);
        return removed;
    }

    public Book? FindByIsbn(string isbn) =>
        _store.Load().FirstOrDefault(b => b.ISBN.Equals(isbn, StringComparison.OrdinalIgnoreCase));
}
