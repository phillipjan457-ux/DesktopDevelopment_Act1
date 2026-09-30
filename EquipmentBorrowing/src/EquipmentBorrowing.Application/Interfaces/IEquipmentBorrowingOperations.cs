using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Interfaces;

public interface IEquipmentBorrowingOperations
{
    Task<IEnumerable<Student>> GetStudentsAsync(
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Equipment>> GetEquipmentAsync(
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Borrowing>> GetActiveBorrowingsAsync(
        CancellationToken cancellationToken = default);

    Task<BorrowResult> BorrowAsync(
        string studentId,
        string equipmentId,
        CancellationToken cancellationToken = default);

    Task<ReturnResult> ReturnAsync(
        string borrowId,
        CancellationToken cancellationToken = default);
}