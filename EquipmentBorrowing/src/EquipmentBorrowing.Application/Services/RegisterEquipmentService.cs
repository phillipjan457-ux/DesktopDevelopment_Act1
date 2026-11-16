using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Services;

public sealed class RegisterEquipmentService
{
    private readonly IEquipmentRepository _equipment;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterEquipmentService(
        IEquipmentRepository equipment,
        IUnitOfWork unitOfWork)
    {
        _equipment = equipment;
        _unitOfWork = unitOfWork;
    }

    public async Task<EquipmentRegistrationResult> RegisterAsync(
        string equipmentId,
        string name,
        string? description,
        CancellationToken cancellationToken = default)
    {
        var normalizedId = equipmentId.Trim().ToUpperInvariant();
        var normalizedName = name.Trim();
        var normalizedDescription = string.IsNullOrWhiteSpace(description)
            ? null
            : description.Trim();

        if (normalizedId.Length == 0 || normalizedName.Length == 0)
        {
            return new EquipmentRegistrationResult
            {
                IsSuccess = false,
                Message = "Equipment ID and name are required."
            };
        }

        if (await _equipment.GetEquipmentByIdAsync(
                normalizedId, cancellationToken) is not null)
        {
            return new EquipmentRegistrationResult
            {
                IsSuccess = false,
                Message = "That equipment ID is already registered."
            };
        }

        var item = new Equipment
        {
            EquipmentId = normalizedId,
            Name = normalizedName,
            Type = null,
            Description = normalizedDescription
        };

        await _equipment.AddEquipmentAsync(item, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new EquipmentRegistrationResult
        {
            IsSuccess = true,
            Message = "Equipment registered.",
            Equipment = item
        };
    }
}