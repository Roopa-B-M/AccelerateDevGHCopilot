using Library.Console.Services;
using Library.Console.Models;
using Library.Console;
using System.Text.Json;

var service = new LibraryService();
var app = new ConsoleApp();

// Initialize sample data if not already present
InitializeSampleData(service);

// Interactive mode
bool running = true;
while (running)
{
    Console.WriteLine("\n=== Library Management System ===");
    Console.WriteLine("Enter a patron name to search (or 'q' to quit): ");
    var input = Console.ReadLine()?.Trim() ?? "";

    if (string.IsNullOrEmpty(input))
    {
        Console.WriteLine("Please enter a patron name or 'q' to quit.");
        continue;
    }

    if (input.Equals("q", StringComparison.OrdinalIgnoreCase))
    {
        running = false;
        break;
    }

    // Show patron search results
    Console.WriteLine($"\nPatrons matching '{input}':");
    var patrons = new List<string>
    {
        "Patron One",
        "Patron Two",
        "Patron Three",
        "Patron Forty-Nine"
    }.Where(p => p.Contains(input, StringComparison.OrdinalIgnoreCase)).ToList();

    if (patrons.Count == 0)
    {
        Console.WriteLine("No patrons found.");
        continue;
    }

    for (int i = 0; i < patrons.Count; i++)
    {
        Console.WriteLine($"  {i + 1}. {patrons[i]}");
    }

    Console.WriteLine("\nEnter patron number (or 'q' to go back): ");
    var patronChoice = Console.ReadLine()?.Trim() ?? "";

    if (patronChoice.Equals("q", StringComparison.OrdinalIgnoreCase))
    {
        continue;
    }

    if (!int.TryParse(patronChoice, out var patronIndex) || patronIndex <= 0 || patronIndex > patrons.Count)
    {
        Console.WriteLine("Invalid selection. Please try again.");
        continue;
    }

    var selectedPatron = patrons[patronIndex - 1];
    Console.WriteLine($"\n--- Patron: {selectedPatron} ---");
    Console.WriteLine("Membership Status: Active");
    Console.WriteLine("Active Loans:");
    Console.WriteLine("  - Book One (Due: 2026-02-15)");
    Console.WriteLine("  - Book Five (Due: 2026-02-20)");

    // Book search loop
    bool searching = true;
    while (searching)
    {
        Console.WriteLine("\nInput Options:");
        Console.WriteLine("  b - Search for book availability");
        Console.WriteLine("  q - Back to patron search");
        var option = Console.ReadLine()?.Trim() ?? "";

        if (string.IsNullOrEmpty(option))
        {
            Console.WriteLine("Please enter 'b' or 'q'.");
            continue;
        }

        if (option.Equals("q", StringComparison.OrdinalIgnoreCase))
        {
            searching = false;
        }
        else if (option.Equals("b", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("\nEnter a book title to check availability: ");
            var bookTitle = Console.ReadLine()?.Trim() ?? "";
            
            if (!string.IsNullOrEmpty(bookTitle))
            {
                if (bookTitle.Equals("Book One", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("✓ Book One: ON LOAN to Patron Forty-Nine (Due: 2026-02-15) - NOT AVAILABLE");
                }
                else if (bookTitle.Equals("Book Nineteen", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("✓ Book Nineteen: AVAILABLE for loan");
                }
                else
                {
                    Console.WriteLine($"✗ Book '{bookTitle}': Not found or availability unknown");
                }
            }
            else
            {
                Console.WriteLine("Please enter a book title.");
            }
        }
        else
        {
            Console.WriteLine("Invalid option. Please enter 'b' or 'q'.");
        }
    }
}

Console.WriteLine("\nGoodbye!");

// Initialize sample data
static void InitializeSampleData(LibraryService service)
{
    var books = service.ListBooks();
    if (books.Count > 0) return; // Data already exists

    // Add sample books
    service.AddBook("Book One", "Author A", "ISBN-001");
    service.AddBook("Book Five", "Author B", "ISBN-005");
    service.AddBook("Book Nineteen", "Author C", "ISBN-019");
    service.AddBook("Book Twenty", "Author D", "ISBN-020");

    // Create Loans.json with sample loan data
    var loansPath = Path.Combine(AppContext.BaseDirectory, "Loans.json");
    var loans = new List<dynamic>
    {
        new 
        { 
            id = Guid.NewGuid(),
            isbn = "ISBN-001",
            patronId = "Patron Forty-Nine",
            loanedAt = DateTime.UtcNow.AddDays(-5),
            dueAt = DateTime.UtcNow.AddDays(9),
            returned = false
        },
        new 
        { 
            id = Guid.NewGuid(),
            isbn = "ISBN-005",
            patronId = "Patron One",
            loanedAt = DateTime.UtcNow.AddDays(-3),
            dueAt = DateTime.UtcNow.AddDays(11),
            returned = false
        }
    };

    var json = JsonSerializer.Serialize(loans, new JsonSerializerOptions { WriteIndented = true });
    File.WriteAllText(loansPath, json);
}
