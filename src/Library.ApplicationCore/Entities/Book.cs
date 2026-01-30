namespace Library.ApplicationCore.Entities
{
    using System;

    public class Book
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string ISBN { get; set; } = string.Empty;
    }
}
