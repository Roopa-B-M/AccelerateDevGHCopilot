namespace Library.ApplicationCore.Services
{
    using System;
    using Library.ApplicationCore.Entities;
    using Library.ApplicationCore.Interfaces;
    using Library.ApplicationCore.Enums;

    public enum LoanReturnStatus
    {
        Success,
        LoanNotFound,
        AlreadyReturned
    }

    public class LoanService
    {
        public const int ExtendByDays = 14;
        private readonly ILoanRepository _loans;

        public LoanService(ILoanRepository loans)
        {
            _loans = loans;
        }

        public Loan ReturnLoan(Loan loan)
        {
            loan.Returned = true;
            loan.ReturnedAt = DateTime.UtcNow;
            _loans.Update(loan);
            return loan;
        }

        public Loan ExtendLoan(Loan loan, int extraDays)
        {
            if (extraDays <= 0) return loan;
            loan.DueDate = ((DateTime)loan.DueDate).AddDays(extraDays);
            _loans.Update(loan);
            return loan;
        }

        public async Task<LoanReturnStatus> ReturnLoan(Guid loanId)
        {
            var loan = await _loans.GetLoanAsync(loanId);
            
            if (loan == null)
                return LoanReturnStatus.LoanNotFound;
            
            if (loan.Returned)
                return LoanReturnStatus.AlreadyReturned;
            
            // Set the return status and timestamp
            loan.Returned = true;
            loan.ReturnedAt = DateTime.UtcNow;
            _loans.Update(loan);
            
            return LoanReturnStatus.Success;
        }

        public async Task<LoanExtensionStatus> ExtendLoan(Guid loanId)
        {
            var loan = await _loans.GetLoanAsync(loanId);
            if (loan == null)
                return LoanExtensionStatus.LoanNotFound;

            if (loan.Returned || loan.ReturnedAt != null)
                return LoanExtensionStatus.LoanReturned;

            if (((DateTime)loan.DueDate) < DateTime.UtcNow)
                return LoanExtensionStatus.LoanExpired;

            if (loan.Patron?.MembershipEnd < DateTime.UtcNow)
                return LoanExtensionStatus.MembershipExpired;

            loan.DueDate = loan.DueDate.AddDays(ExtendByDays);
            _loans.Update(loan);
            return LoanExtensionStatus.Success;
        }
    }
}

namespace Library.ApplicationCore.Enums
{
    public enum LoanExtensionStatus
    {
        Success,
        LoanNotFound,
        MembershipExpired,
        LoanReturned,
        LoanExpired
    }
}
