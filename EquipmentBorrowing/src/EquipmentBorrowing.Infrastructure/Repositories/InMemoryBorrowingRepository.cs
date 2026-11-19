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

    public Task<Borrowing?> GetBorrowingByIdAsync(string borrowId, CancellationToken cancellationToken = default)
    {
        var borrowing = _borrowings.FirstOrDefault(b => b.BorrowId == borrowId);
        return Task.FromResult(borrowing);
    }

    public Task<IEnumerable<Borrowing>> ListActiveBorrowings(CancellationToken cancellationToken = default)
    {
        var active = _borrowings.Where(b => b.Status == BorrowingStatus.Active);
        return Task.FromResult(active);
    }
    public Task<IEnumerable<Borrowing>> ListByEquipmentAsync(
    string equipmentId,
    CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var borrowings = _borrowings
            .Where(borrowing => borrowing.EquipmentId == equipmentId)
            .OrderByDescending(borrowing => borrowing.BorrowDate)
            .ThenBy(borrowing => borrowing.BorrowId)
            .ToList();

        return Task.FromResult<IEnumerable<Borrowing>>(borrowings);
    }
    public Task<IEnumerable<Borrowing>> ListAllBorrowings(
    CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IEnumerable<Borrowing> borrowings = _borrowings
            .OrderByDescending(borrowing => borrowing.BorrowDate)
            .ThenBy(borrowing => borrowing.BorrowId)
            .ToList();

        return Task.FromResult(borrowings);
    }
    public Task UpdateStudentIdAsync(
    string oldStudentId,
    Student updatedStudent,
    CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        foreach (var borrowing in _borrowings.Where(
                     borrowing => borrowing.StudentId == oldStudentId))
        {
            borrowing.StudentId = updatedStudent.StudentId;
            borrowing.Student = updatedStudent;
        }

        return Task.CompletedTask;
    }
}