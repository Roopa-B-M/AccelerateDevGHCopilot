using NSubstitute;
using Library.ApplicationCore;
using Library.ApplicationCore.Entities;
using Library.ApplicationCore.Enums;
using Library.ApplicationCore.Interfaces;
using Library.ApplicationCore.Services;

namespace Library.UnitTests.ApplicationCore.LoanServiceTests;

public class ReturnLoanTest
{
    private readonly Library.ApplicationCore.Interfaces.ILoanRepository _mockLoanRepository;
    private readonly Library.ApplicationCore.Services.LoanService _loanService;

    public ReturnLoanTest()
    {
        _mockLoanRepository = NSubstitute.Substitute.For<Library.ApplicationCore.Interfaces.ILoanRepository>();
        _loanService = new Library.ApplicationCore.Services.LoanService(_mockLoanRepository);
    }

    [Fact(DisplayName = "LoanService.ReturnLoan: Returns LoanNotFound if loan is not found")]
    public async Task ReturnLoan_ReturnsLoanNotFound()
    {
        // Arrange
        var loanId = Guid.NewGuid();
        _mockLoanRepository.GetLoanAsync(loanId).Returns((Loan?)null);

        // Act
        LoanReturnStatus returnStatus = await _loanService.ReturnLoan(loanId);

        // Assert
        Assert.Equal(LoanReturnStatus.LoanNotFound, returnStatus);
    }

    [Fact(DisplayName = "LoanService.ReturnLoan: Returns AlreadyReturned if loan is already returned")]
    public async Task ReturnLoan_ReturnsAlreadyReturned()
    {
        // Arrange
        var patron = PatronFactory.CreateCurrentPatron();
        var loan = LoanFactory.CreateReturnedLoanForPatron(patron);
        var loanId = loan.Id;
        _mockLoanRepository.GetLoanAsync(loanId).Returns(loan);

        // Act
        LoanReturnStatus returnStatus = await _loanService.ReturnLoan(loanId);

        // Assert
        Assert.Equal(LoanReturnStatus.AlreadyReturned, returnStatus);
    }

    [Fact(DisplayName = "LoanService.ReturnLoan: Returns Success and updates return date for a patron with current membership")]
    public async Task ReturnLoan_ReturnsSuccessAndUpdateReturnDate()
    {
        // Arrange
        var patron = PatronFactory.CreateCurrentPatron();
        var loan = LoanFactory.CreateCurrentLoanForPatron(patron);
        var loanId = loan.Id;
        _mockLoanRepository.GetLoanAsync(loanId).Returns(loan);

        // Act
        LoanReturnStatus returnStatus = await _loanService.ReturnLoan(loanId);

        // Assert
        Assert.Equal(LoanReturnStatus.Success, returnStatus);
        Assert.NotNull(loan.ReturnedAt);
    }

    [Fact(DisplayName = "LoanService.ReturnLoan: Returns Success and updates return date for an expired loan")]
    public async Task ReturnLoan_ReturnsSuccessAndUpdateReturnDateForExpiredLoan()
    {
        // Arrange
        var patron = PatronFactory.CreateCurrentPatron();
        var loan = LoanFactory.CreateExpiredLoanForPatron(patron);
        var loanId = loan.Id;
        _mockLoanRepository.GetLoanAsync(loanId).Returns(loan);

        // Act
        LoanReturnStatus returnStatus = await _loanService.ReturnLoan(loanId);

        // Assert
        Assert.Equal(LoanReturnStatus.Success, returnStatus);
        Assert.NotNull(loan.ReturnedAt);
    }

    [Fact(DisplayName = "LoanService.ReturnLoan: Returns Success and updates return date for a patron with expired membership")]
    public async Task ReturnLoan_ReturnsSuccessAndUpdateReturnDateForExpiredPatron()
    {
        // Arrange
        var loan = LoanFactory.CreateCurrentLoanForPatron(PatronFactory.CreateExpiredPatron());
        var loanId = loan.Id;
        _mockLoanRepository.GetLoanAsync(loanId).Returns(loan);

        // Act
        LoanReturnStatus returnStatus = await _loanService.ReturnLoan(loanId);

        // Assert
        Assert.Equal(LoanReturnStatus.Success, returnStatus);
        Assert.NotNull(loan.ReturnedAt);
    }
}

