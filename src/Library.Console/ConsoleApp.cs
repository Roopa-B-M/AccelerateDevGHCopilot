global using Console = System.Console;

using System;
using System.Text.Json;
using SystemConsole = System.Console;

namespace Library.Console;

public class ConsoleApp
{
    private readonly string _libraryPath = Path.Combine(AppContext.BaseDirectory, "library.json");
    private readonly string _loansPath = Path.Combine(AppContext.BaseDirectory, "Loans.json");
    private readonly string _reservationsPath = Path.Combine(AppContext.BaseDirectory, "Reservations.json");

    private readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web) { WriteIndented = true };

    // Lightweight models for local persistence
    private record BookLite(Guid Id, string Title, string Author, string ISBN);
    private record Loan(Guid Id, string ISBN, string PatronId, DateTime LoanedAt, DateTime DueAt, bool Returned);
    private record Reservation(Guid Id, string ISBN, string PatronId, DateTime ReservedAt);

    public void PrintAvailability(string isbn)
    {
        var loan = Load<Loan>(_loansPath).FirstOrDefault(l => IsIsbn(l.ISBN, isbn) && !l.Returned);
        if (loan is not null)
        {
            SystemConsole.WriteLine($"On loan. Due date: {loan.DueAt:d}");
            return;
        }
        SystemConsole.WriteLine("Available for loan.");
    }

    public void LoanBook(string isbn, string patronId, int days)
    {
        var book = Load<BookLite>(_libraryPath).FirstOrDefault(b => IsIsbn(b.ISBN, isbn));
        if (book is null)
        {
            SystemConsole.WriteLine("Book not found.");
            return;
        }

        var loans = Load<Loan>(_loansPath);
        var active = loans.FirstOrDefault(l => IsIsbn(l.ISBN, isbn) && !l.Returned);
        if (active is not null)
        {
            SystemConsole.WriteLine($"Not available. Current loan due: {active.DueAt:d}");
            return;
        }

        var loan = new Loan(Guid.NewGuid(), isbn, patronId, DateTime.UtcNow, DateTime.UtcNow.AddDays(days), false);
        loans.Add(loan);
        Save(_loansPath, loans);

        SystemConsole.WriteLine($"Loan created: \"{book.Title}\" ({book.ISBN}) to patron {patronId}. Due: {loan.DueAt:d}");
        var patronLoans = loans.Where(l => l.PatronId.Equals(patronId, StringComparison.OrdinalIgnoreCase) && !l.Returned).ToList();
        SystemConsole.WriteLine($"Active loans for {patronId}:");
        foreach (var pl in patronLoans)
            SystemConsole.WriteLine($"- {pl.ISBN} due {pl.DueAt:d}");
    }

    public void ReserveBook(string isbn, string patronId)
    {
        var book = Load<BookLite>(_libraryPath).FirstOrDefault(b => IsIsbn(b.ISBN, isbn));
        if (book is null)
        {
            SystemConsole.WriteLine("Book not found.");
            return;
        }

        var reservations = Load<Reservation>(_reservationsPath);
        var existing = reservations.FirstOrDefault(r => IsIsbn(r.ISBN, isbn));
        if (existing is not null)
        {
            SystemConsole.WriteLine("Already reserved.");
            return;
        }

        var reservation = new Reservation(Guid.NewGuid(), isbn, patronId, DateTime.UtcNow);
        reservations.Add(reservation);
        Save(_reservationsPath, reservations);

        SystemConsole.WriteLine($"Reserved: \"{book.Title}\" ({book.ISBN}) for patron {patronId}.");
    }

    private static bool IsIsbn(string a, string b) =>
        a.Equals(b, StringComparison.OrdinalIgnoreCase);

    private List<T> Load<T>(string path)
    {
        if (!File.Exists(path)) return new List<T>();
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<List<T>>(json, _json) ?? new List<T>();
    }

    private void Save<T>(string path, List<T> items)
    {
        var json = JsonSerializer.Serialize(items, _json);
        File.WriteAllText(path, json);
    }
}
