namespace Library.ApplicationCore.Entities
{
    using System;
    using System.Collections.Generic;
    using Library.ApplicationCore.Enums;

    public class Patron
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public DateTime MembershipExpiresOn { get; set; }
        public DateTime? MembershipEnd { get; set; }
        public MembershipStatus Status { get; set; } = MembershipStatus.Active;
        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}
