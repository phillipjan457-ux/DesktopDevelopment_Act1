namespace EquipmentBorrowing.Application.Interfaces;

using EquipmentBorrowing.Domain;
using System.Threading;
public interface IBorrowingRepository
{
    Task<Borrowing?> SaveBorrowing(Borrowing borrowing, CancellationToken cancellationToken = default);
    Task<IEnumerable<Borrowing>> ListOfBorrows(string studentId, CancellationToken cancellationToken = default);
    Task<Borrowing?> GetBorrowingByIdAsync(string borrowId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Borrowing>> ListActiveBorrowings(CancellationToken cancellationToken = default);
}