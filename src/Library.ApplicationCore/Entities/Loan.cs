namespace Library.ApplicationCore.Entities
{
    using System;

    public class Loan
    {
        public Guid Id { get; set; }
        public string ISBN { get; set; } = string.Empty;
        public Guid PatronId { get; set; }
        public Patron? Patron { get; set; } // Navigation property
        public DateTime LoanedAt { get; set; }
        public DateTime DueDate { get; set; }
        public bool Returned { get; set; }
        public DateTime? ReturnedAt { get; set; }
    }
}
