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
    Task<MultiCheckoutResult> CheckoutMultipleAsync(
    string studentId,
    IReadOnlyCollection<string> equipmentIds,
    DateOnly expectedReturnDate,
    CancellationToken cancellationToken = default);
    Task<ReturnResult> ReturnAsync(
        string borrowId,
        CancellationToken cancellationToken = default);
    Task<StudentRegistrationResult> RegisterStudentAsync(
    string studentId,
    string name,
    string? contact,
    CancellationToken cancellationToken = default);

    Task<EquipmentRegistrationResult> RegisterEquipmentAsync(
    string equipmentId,
    string name,
    string? description,
    CancellationToken cancellationToken = default);
    Task<EquipmentUpdateResult> UpdateEquipmentAsync(
    string equipmentId,
    string name,
    string? description,
    CancellationToken cancellationToken = default);
    Task<StudentUpdateResult> UpdateStudentAsync(
    string studentId,
    string name,
    string? program,
    string? contact,
    bool isAuthorized,
    CancellationToken cancellationToken = default);
    Task<IEnumerable<Borrowing>> GetStudentBorrowingHistoryAsync(
    string studentId,
    CancellationToken cancellationToken = default);
    Task<IEnumerable<Equipment>> GetAllEquipmentAsync(
    CancellationToken cancellationToken = default);

    Task<IEnumerable<Borrowing>> GetEquipmentBorrowingHistoryAsync(
        string equipmentId,
        CancellationToken cancellationToken = default);
}