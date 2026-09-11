namespace EquipmentBorrowing.Application.Services;

using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

public class ReturnEquipmentService
{
    private readonly IBorrowingRepository _borrowingRepository;
    private readonly IEquipmentRepository _equipmentRepository;

    public ReturnEquipmentService(IBorrowingRepository borrowingRepository, IEquipmentRepository equipmentRepository)
    {
        _borrowingRepository = borrowingRepository;
        _equipmentRepository = equipmentRepository;
    }

    public async Task<ReturnResult> ReturnEquipmentAsync(string borrowId, CancellationToken cancellationToken = default)
    {
        var borrowing = await _borrowingRepository.GetBorrowingByIdAsync(borrowId, cancellationToken);

        if (borrowing is null)
        {
            return new ReturnResult
            {
                IsSuccess = false,
                Message = "Borrowing record not found."
            };
        }

        if (borrowing.Status != BorrowingStatus.Active)
        {
            return new ReturnResult
            {
                IsSuccess = false,
                Message = "This equipment has already been returned."
            };
        }

        borrowing.Status = BorrowingStatus.Returned;

        borrowing.Equipment.MarkAsAvailable();
        await _equipmentRepository.SaveEquipmentAsync(borrowing.Equipment, cancellationToken);

        return new ReturnResult
        {
            IsSuccess = true,
            Message = "Equipment returned successfully.",
            Borrowing = borrowing
        };
    }
}