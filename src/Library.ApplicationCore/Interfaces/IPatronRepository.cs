namespace Library.ApplicationCore.Interfaces
{
    using System;
    using Library.ApplicationCore.Entities;

    public interface IPatronRepository
    {
        Patron? GetById(Guid id);
        void Update(Patron patron);
    }
}
