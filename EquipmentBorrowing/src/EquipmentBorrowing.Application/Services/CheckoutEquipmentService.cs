using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Services;

public sealed class CheckoutEquipmentService
{
    private const int MaxActiveItems = 3;

    private readonly IStudentRepository _students;
    private readonly IEquipmentRepository _equipment;
    private readonly IBorrowingRepository _borrowings;
    private readonly IUnitOfWork _unitOfWork;

    public CheckoutEquipmentService(
        IStudentRepository students,
        IEquipmentRepository equipment,
        IBorrowingRepository borrowings,
        IUnitOfWork unitOfWork)
    {
        _students = students;
        _equipment = equipment;
        _borrowings = borrowings;
        _unitOfWork = unitOfWork;
    }

    public async Task<MultiCheckoutResult> CheckoutAsync(
        string studentId,
        IReadOnlyCollection<string> equipmentIds,
        DateOnly expectedReturnDate,
        CancellationToken cancellationToken = default)
    {
        MultiCheckoutResult? result = null;

        await _unitOfWork.ExecuteInTransactionAsync(async token =>
        {
            var normalizedIds = equipmentIds
                .Select(id => id.Trim().ToUpperInvariant())
                .ToArray();

            if (normalizedIds.Length < 2)
            {
                result = Rejected("Select at least two equipment items.");
                return;
            }

            if (normalizedIds.Any(string.IsNullOrWhiteSpace))
            {
                result = Rejected("Every selected item must have an equipment ID.");
                return;
            }

            if (normalizedIds.Distinct(StringComparer.Ordinal).Count()
                != normalizedIds.Length)
            {
                result = Rejected("The same equipment item cannot be selected twice.");
                return;
            }

            if (expectedReturnDate <= DateOnly.FromDateTime(DateTime.Now))
            {
                result = Rejected("Expected return date must be later than today.");
                return;
            }

            var student = await _students.GetStudentByIdAsync(
                studentId.Trim().ToUpperInvariant(),
                token);

            if (student is null)
            {
                result = Rejected("Student not found.");
                return;
            }

            if (!student.IsAuthorized)
            {
                result = Rejected("Student is not authorized to borrow equipment.");
                return;
            }

            var existingBorrowings = await _borrowings.ListOfBorrows(
                student.StudentId,
                token);

            var activeCount = existingBorrowings.Count(
                borrowing => borrowing.Status == BorrowingStatus.Active);

            if (activeCount + normalizedIds.Length > MaxActiveItems)
            {
                result = Rejected("The student would exceed the limit of three active items.");
                return;
            }

            var selectedItems = new List<Equipment>();

            foreach (var equipmentId in normalizedIds)
            {
                var item = await _equipment.GetEquipmentByIdAsync(
                    equipmentId,
                    token);

                if (item is null)
                {
                    result = Rejected($"Equipment '{equipmentId}' was not found.");
                    return;
                }

                if (item.IsActivelyBorrowed)
                {
                    result = Rejected($"{item.Name} is already being borrowed.");
                    return;
                }

                selectedItems.Add(item);
            }

            var borrowDate = DateTime.UtcNow;
            var returnDate = DateTime.SpecifyKind(
                expectedReturnDate.ToDateTime(new TimeOnly(12, 0)),
                DateTimeKind.Utc);

            if (returnDate <= borrowDate)
            {
                result = Rejected("Expected return date must be later than checkout.");
                return;
            }

            var newBorrowings = new List<Borrowing>();

            foreach (var item in selectedItems)
            {
                var borrowing = new Borrowing
                {
                    BorrowId = Guid.NewGuid().ToString(),
                    StudentId = student.StudentId,
                    Student = student,
                    EquipmentId = item.EquipmentId,
                    Equipment = item,
                    BorrowDate = borrowDate,
                    ReturnDate = returnDate
                };

                await _borrowings.SaveBorrowing(borrowing, token);
                item.MarkAsBorrowed();
                await _equipment.SaveEquipmentAsync(item, token);
                newBorrowings.Add(borrowing);
            }

            await _unitOfWork.SaveChangesAsync(token);

            result = new MultiCheckoutResult
            {
                IsSuccess = true,
                Message = $"{newBorrowings.Count} equipment items checked out.",
                Borrowings = newBorrowings
            };
        }, cancellationToken);

        return result ?? Rejected("Checkout did not complete.");
    }

    private static MultiCheckoutResult Rejected(string message)
    {
        return new MultiCheckoutResult
        {
            IsSuccess = false,
            Message = message
        };
    }
}