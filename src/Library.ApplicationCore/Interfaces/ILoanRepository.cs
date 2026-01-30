namespace Library.ApplicationCore.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Library.ApplicationCore.Entities;

    public interface ILoanRepository
    {
        Loan? GetById(Guid id);
        IEnumerable<Loan> GetByPatronId(Guid patronId);
        Loan? GetActiveLoanByIsbn(string isbn);
        void Add(Loan loan);
        Task<Loan?> GetLoanAsync(Guid loanId);
        void Update(Loan loan);
    }
}
