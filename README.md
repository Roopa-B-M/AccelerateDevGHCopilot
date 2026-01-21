# AccelerateDevGHCopilot - Library Management System

A robust .NET 9.0 console application demonstrating clean architecture principles with a complete library management system. This project showcases best practices in layered architecture, dependency injection, repository pattern, and unit testing.

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Core Entities](#core-entities)
- [Services](#services)
- [Data Access](#data-access)
- [Testing](#testing)
- [Getting Started](#getting-started)
- [Technology Stack](#technology-stack)

## 🎯 Overview

**AccelerateDevGHCopilot** is a library management system that enables users to:
- Manage library patrons and their memberships
- Track book loans and returns
- Extend loan due dates
- Renew patron memberships
- Handle business rule validation for all operations

The application uses JSON-based data storage and implements a clean, testable architecture with clear separation of concerns.

## ✨ Features

### Patron Management
- **Search Patrons** - Find patrons by name
- **View Details** - Display patron information and membership status
- **Renew Membership** - Extend patron memberships with business rule validation
- **Loan Tracking** - View all loans associated with a patron

### Loan Management
- **View Loans** - Browse active and returned loans
- **Return Books** - Process book returns with status tracking
- **Extend Loans** - Extend due dates with validation rules
- **Track Overdue** - Identify overdue loans

### Business Rule Validation

| Operation | Validation Rules |
|-----------|------------------|
| **Membership Renewal** | ✓ Only within 1 month before expiration<br>✓ Blocked if patron has overdue loans |
| **Loan Extension** | ✓ Blocked if membership expired<br>✓ Blocked if loan already returned<br>✓ Blocked if loan past due<br>✓ Extends by 14 days |
| **Loan Return** | ✓ Allowed at any time<br>✓ Prevents duplicate returns |

## 🏗️ Architecture

The project follows **Clean Architecture** principles with three distinct layers:

```
┌─────────────────────────────────────┐
│   Library.Console (Presentation)    │ ← User Interface
├─────────────────────────────────────┤
│  Library.ApplicationCore (Business) │ ← Services & Interfaces
├─────────────────────────────────────┤
│ Library.Infrastructure (Data)       │ ← Repositories & Data Access
└─────────────────────────────────────┘
```

### Layer Responsibilities

**Library.Console** (Presentation Layer)
- Console UI and user interaction
- State machine implementation
- Menu navigation
- Configuration management

**Library.ApplicationCore** (Business Logic Layer)
- Domain entities and models
- Business rules and validation
- Service interfaces and implementations
- Status enumerations

**Library.Infrastructure** (Data Access Layer)
- JSON-based repositories
- Data persistence and retrieval
- Entity mapping and serialization

## 📁 Project Structure

```
AccelerateDevGHCopilot/
├── src/
│   ├── Library.ApplicationCore/
│   │   ├── Entities/
│   │   │   ├── Author.cs
│   │   │   ├── Book.cs
│   │   │   ├── BookItem.cs
│   │   │   ├── Loan.cs
│   │   │   └── Patron.cs
│   │   ├── Enums/
│   │   │   ├── EnumHelper.cs
│   │   │   ├── LoanExtensionStatus.cs
│   │   │   ├── LoanReturnStatus.cs
│   │   │   └── MembershipRenewalStatus.cs
│   │   ├── Interfaces/
│   │   │   ├── ILoanRepository.cs
│   │   │   ├── ILoanService.cs
│   │   │   ├── IPatronRepository.cs
│   │   │   └── IPatronService.cs
│   │   ├── Services/
│   │   │   ├── LoanService.cs
│   │   │   └── PatronService.cs
│   │   └── Library.ApplicationCore.csproj
│   ├── Library.Console/
│   │   ├── Json/
│   │   │   ├── Authors.json
│   │   │   ├── Books.json
│   │   │   ├── BookItems.json
│   │   │   ├── Loans.json
│   │   │   └── Patrons.json
│   │   ├── appSettings.json
│   │   ├── CommonActions.cs
│   │   ├── ConsoleApp.cs
│   │   ├── ConsoleState.cs
│   │   ├── Program.cs
│   │   └── Library.Console.csproj
│   ├── Library.Infrastructure/
│   │   ├── Data/
│   │   │   ├── JsonData.cs
│   │   │   ├── JsonLoanRepository.cs
│   │   │   └── JsonPatronRepository.cs
│   │   └── Library.Infrastructure.csproj
│   └── AccelerateDevGHCopilot.sln
├── tests/
│   └── UnitTests/
│       ├── ApplicationCore/
│       │   ├── LoanService/
│       │   │   ├── ExtendLoan.cs
│       │   │   └── ReturnLoan.cs
│       │   └── PatronService/
│       │       └── RenewMembership.cs
│       ├── LoanFactory.cs
│       ├── PatronFactory.cs
│       └── UnitTests.csproj
└── README.md
```

## 📦 Core Entities

### Patron
Represents a library member with membership information.

| Property | Type | Description |
|----------|------|-------------|
| `Id` | int | Unique identifier |
| `Name` | string | Patron's full name (required) |
| `ImageName` | string | Reference to patron's image |
| `MembershipStart` | DateTime | Membership activation date |
| `MembershipEnd` | DateTime | Membership expiration date |
| `Loans` | List<Loan> | Navigation property to patron's loans |

### Loan
Tracks book loans and returns.

| Property | Type | Description |
|----------|------|-------------|
| `Id` | int | Unique identifier |
| `BookItemId` | int | Reference to loaned book copy |
| `PatronId` | int | Reference to borrowing patron |
| `LoanDate` | DateTime | Date loan was issued |
| `DueDate` | DateTime | Return due date |
| `ReturnDate` | DateTime? | Actual return date (null if not returned) |
| `BookItem` | BookItem | Navigation property |
| `Patron` | Patron | Navigation property |

### Book
Represents a book title in the library catalog.

| Property | Type | Description |
|----------|------|-------------|
| `Id` | int | Unique identifier |
| `Title` | string | Book title (required) |
| `ISBN` | string | ISBN number (required) |
| `Genre` | string | Book genre (required) |
| `AuthorId` | int | Reference to author |
| `ImageName` | string | Book cover image reference (required) |
| `Author` | Author | Navigation property |

### BookItem
Represents a physical copy of a book.

| Property | Type | Description |
|----------|------|-------------|
| `Id` | int | Unique identifier |
| `BookId` | int | Reference to book catalog entry |
| `AcquisitionDate` | DateTime | Date item was acquired |
| `Condition` | string | Physical condition of the item |
| `Book` | Book | Navigation property |

### Author
Represents a book author.

| Property | Type | Description |
|----------|------|-------------|
| `Id` | int | Unique identifier |
| `Name` | string | Author's name (required) |

## 🔧 Services

### LoanService
Manages loan operations and business rules.

```csharp
public interface ILoanService
{
    Task<LoanReturnStatus> ReturnLoan(int loanId);
    Task<LoanExtensionStatus> ExtendLoan(int loanId);
}
```

**Key Methods:**
- `ReturnLoan(loanId)` - Returns a loaned book and updates loan status
- `ExtendLoan(loanId)` - Extends loan due date by 14 days with validation

**Status Enums:**
- `LoanReturnStatus` - Success, AlreadyReturned, LoanNotFound
- `LoanExtensionStatus` - Success, LoanNotFound, LoanAlreadyReturned, PatronshipExpired, LoanOverdue

### PatronService
Manages patron membership operations.

```csharp
public interface IPatronService
{
    Task<MembershipRenewalStatus> RenewMembership(int patronId);
}
```

**Key Methods:**
- `RenewMembership(patronId)` - Renews patron membership for 1 year with validation

**Status Enums:**
- `MembershipRenewalStatus` - Success, PatronNotFound, TooEarlyToRenew, PatronHasOverdueLoans

## 💾 Data Access

### Repository Pattern
The infrastructure layer implements the repository pattern for data abstraction.

```csharp
public interface IPatronRepository
{
    Task<IEnumerable<Patron>> GetAllPatrons();
    Task<Patron?> GetPatronById(int id);
    Task<IEnumerable<Patron>> SearchPatrons(string name);
    Task UpdatePatron(Patron patron);
}

public interface ILoanRepository
{
    Task<IEnumerable<Loan>> GetAllLoans();
    Task<Loan?> GetLoanById(int id);
    Task UpdateLoan(Loan loan);
}
```

### JsonData Manager
Central data loader that handles JSON serialization/deserialization with configuration from `appSettings.json`.

**Configuration (appSettings.json):**
```json
{
    "JsonPaths": {
        "Authors": "Json/Authors.json",
        "Books": "Json/Books.json",
        "BookItems": "Json/BookItems.json",
        "Patrons": "Json/Patrons.json",
        "Loans": "Json/Loans.json"
    }
}
```

## 🧪 Testing

The project includes comprehensive unit tests using **xUnit** testing framework with factory classes for test data creation.

### Test Factories

**PatronFactory**
- Creates patrons with various membership states
- Generates patrons with active, expired, and soon-to-expire memberships

**LoanFactory**
- Creates loans in different states
- Generates returned, current, and overdue loans

### Test Suites

| Test Suite | Tests | Purpose |
|-----------|-------|---------|
| `RenewMembership.cs` | PatronService renewal logic | Validates renewal rules and edge cases |
| `ReturnLoan.cs` | LoanService return logic | Ensures proper loan return handling |
| `ExtendLoan.cs` | LoanService extension logic | Validates loan extension rules |

**Example Test Structure:**
```csharp
[Fact]
public async Task RenewMembership_WithValidPatron_ReturnsSuccess()
{
    // Arrange
    var patron = PatronFactory.CreateActivePatron();
    
    // Act
    var result = await _patronService.RenewMembership(patron.Id);
    
    // Assert
    Assert.Equal(MembershipRenewalStatus.Success, result);
}
```

## 🚀 Getting Started

### Prerequisites
- .NET 9.0 SDK or higher
- Visual Studio or VS Code with C# extension

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/AccelerateDevGHCopilot.git
   cd AccelerateDevGHCopilot
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the project**
   ```bash
   dotnet build
   ```

### Running the Application

```bash
cd src/Library.Console
dotnet run
```

### Running Tests

```bash
cd tests/UnitTests
dotnet test
```

## 🛠️ Technology Stack

| Technology | Purpose |
|-----------|---------|
| **.NET 9.0** | Application framework |
| **C# 13** | Programming language |
| **xUnit** | Unit testing framework |
| **Dependency Injection** | Service management (Microsoft.Extensions.DependencyInjection) |
| **Configuration** | Settings management (Microsoft.Extensions.Configuration) |
| **System.Text.Json** | JSON serialization |

## 📝 Design Patterns

- **Repository Pattern** - Data access abstraction
- **Dependency Injection** - Loose coupling and testability
- **Factory Pattern** - Test data creation
- **State Machine** - Console application flow
- **Status Enums** - Operation result tracking

## 🎓 Learning Resources

This project demonstrates:
- Clean Architecture principles
- SOLID design principles (Single Responsibility, Dependency Inversion)
- Proper layering and separation of concerns
- Async/await patterns
- Unit testing best practices
- Dependency injection configuration
- Repository pattern implementation

## 📄 License

This project is part of the GitHub Copilot learning initiative.

## 👤 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

---

**Built with ❤️ using GitHub Copilot**
