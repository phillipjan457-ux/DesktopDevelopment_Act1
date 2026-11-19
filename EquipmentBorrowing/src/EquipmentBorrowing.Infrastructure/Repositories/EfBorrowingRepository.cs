using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;
using EquipmentBorrowing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EquipmentBorrowing.Infrastructure.Repositories;

public class EfBorrowingRepository : IBorrowingRepository
{
    private readonly EquipmentBorrowingDbContext _context;

    public EfBorrowingRepository(EquipmentBorrowingDbContext context)
    {
        _context = context;
    }

    public async Task<Borrowing?> SaveBorrowing(
        Borrowing borrowing,
        CancellationToken cancellationToken = default)
    {
        var existing = await _context.Borrowings
            .SingleOrDefaultAsync(
                item => item.BorrowId == borrowing.BorrowId,
                cancellationToken);

        if (existing is null)
        {
            _context.Borrowings.Add(borrowing);
        }
        else
        {
            _context.Entry(existing).CurrentValues.SetValues(borrowing);
        }

        // The application operation will commit all changes together.
        return existing ?? borrowing;
    }

    public async Task<IEnumerable<Borrowing>> ListOfBorrows(
        string studentId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Borrowings
            .AsNoTracking()
            .Where(borrowing => borrowing.StudentId == studentId)
            .Include(borrowing => borrowing.Student)
            .Include(borrowing => borrowing.Equipment)
            .OrderByDescending(borrowing => borrowing.BorrowDate)
            .ThenBy(borrowing => borrowing.BorrowId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Borrowing?> GetBorrowingByIdAsync(
        string borrowId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Borrowings
            .Include(borrowing => borrowing.Student)
            .Include(borrowing => borrowing.Equipment)
            .SingleOrDefaultAsync(
                borrowing => borrowing.BorrowId == borrowId,
                cancellationToken);
    }

    public async Task<IEnumerable<Borrowing>> ListActiveBorrowings(
        CancellationToken cancellationToken = default)
    {
        return await _context.Borrowings
            .AsNoTracking()
            .Where(borrowing => borrowing.Status == BorrowingStatus.Active)
            .Include(borrowing => borrowing.Student)
            .Include(borrowing => borrowing.Equipment)
            .OrderBy(borrowing => borrowing.ReturnDate)
            .ThenBy(borrowing => borrowing.BorrowId)
            .ToListAsync(cancellationToken);
    }
    public async Task<IEnumerable<Borrowing>> ListByEquipmentAsync(
    string equipmentId,
    CancellationToken cancellationToken = default)
    {
        return await _context.Borrowings
            .AsNoTracking()
            .Where(borrowing => borrowing.EquipmentId == equipmentId)
            .Include(borrowing => borrowing.Student)
            .Include(borrowing => borrowing.Equipment)
            .OrderByDescending(borrowing => borrowing.BorrowDate)
            .ThenBy(borrowing => borrowing.BorrowId)
            .ToListAsync(cancellationToken);
    }
    public async Task<IEnumerable<Borrowing>> ListAllBorrowings(
    CancellationToken cancellationToken = default)
    {
        return await _context.Borrowings
            .AsNoTracking()
            .Include(borrowing => borrowing.Student)
            .Include(borrowing => borrowing.Equipment)
            .OrderByDescending(borrowing => borrowing.BorrowDate)
            .ThenBy(borrowing => borrowing.BorrowId)
            .ToListAsync(cancellationToken);
    }
    public async Task UpdateStudentIdAsync(
    string oldStudentId,
    Student updatedStudent,
    CancellationToken cancellationToken = default)
    {
        await _context.Borrowings
            .Where(borrowing => borrowing.StudentId == oldStudentId)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(
                    borrowing => borrowing.StudentId,
                    updatedStudent.StudentId),
                cancellationToken);
    }
}