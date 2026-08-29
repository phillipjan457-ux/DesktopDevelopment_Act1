namespace EquipmentBorrowing.Infrastructure.Repositories;

using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

public class InMemoryBorrowingRepository : IBorrowingRepository
{
    private readonly List<Borrowing> _borrowings;

    public InMemoryBorrowingRepository()
    {
        _borrowings = new List<Borrowing>();
    }

    public Task<Borrowing?> SaveBorrowing(Borrowing borrowing, CancellationToken cancellationToken = default)
    {
        _borrowings.Add(borrowing);
        return Task.FromResult<Borrowing?>(borrowing);
    }

    public Task<IEnumerable<Borrowing>> ListOfBorrows(string studentId, CancellationToken cancellationToken = default)
    {
        var borrows = _borrowings.Where(b => b.Student.StudentId == studentId);
        return Task.FromResult(borrows);
    }   
}