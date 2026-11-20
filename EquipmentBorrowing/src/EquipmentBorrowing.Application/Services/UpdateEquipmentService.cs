using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Services;

public sealed class UpdateEquipmentService
{
    private readonly IEquipmentRepository _equipment;
    private readonly IBorrowingRepository _borrowings;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateEquipmentService(
        IEquipmentRepository equipment,
        IBorrowingRepository borrowings,
        IUnitOfWork unitOfWork)
    {
        _equipment = equipment;
        _borrowings = borrowings;
        _unitOfWork = unitOfWork;
    }

    public async Task<EquipmentUpdateResult> UpdateAsync(
        string equipmentId,
        string newEquipmentId,
        string name,
        string? description,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(equipmentId) ||
            string.IsNullOrWhiteSpace(newEquipmentId) ||
            string.IsNullOrWhiteSpace(name))
        {
            return Rejected("Equipment ID and name are required.");
        }

        var oldId = equipmentId.Trim().ToUpperInvariant();
        var newId = newEquipmentId.Trim().ToUpperInvariant();
        EquipmentUpdateResult? result = null;

        await _unitOfWork.ExecuteInTransactionAsync(async token =>
        {
            var current = await _equipment.GetEquipmentByIdAsync(oldId, token);

            if (current is null)
            {
                result = Rejected("Equipment was not found.");
                return;
            }

            if (newId != oldId &&
                await _equipment.GetEquipmentByIdAsync(newId, token) is not null)
            {
                result = Rejected("That equipment ID is already in use.");
                return;
            }

            var normalizedDescription =
                string.IsNullOrWhiteSpace(description) ? null : description.Trim();

            if (newId == oldId)
            {
                current.Name = name.Trim();
                current.Description = normalizedDescription;
                await _equipment.SaveEquipmentAsync(current, token);
            }
            else
            {
                var replacement = new Equipment
                {
                    EquipmentId = newId,
                    Name = name.Trim(),
                    Type = current.Type,
                    Description = normalizedDescription
                };

                if (current.IsActivelyBorrowed)
                {
                    replacement.MarkAsBorrowed();
                }

                // Save the new equipment before moving the borrowing references.
                await _equipment.AddEquipmentAsync(replacement, token);
                await _unitOfWork.SaveChangesAsync(token);

                await _borrowings.UpdateEquipmentIdAsync(oldId, replacement, token);
                await _equipment.DeleteEquipmentAsync(current, token);
            }

            await _unitOfWork.SaveChangesAsync(token);

            result = new EquipmentUpdateResult
            {
                IsSuccess = true,
                Message = "Equipment updated."
            };
        }, cancellationToken);

        return result ?? Rejected("Equipment update did not complete.");
    }

    private static EquipmentUpdateResult Rejected(string message) =>
        new()
        {
            IsSuccess = false,
            Message = message
        };
}