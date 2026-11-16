using EquipmentBorrowing.Application.Interfaces;

namespace EquipmentBorrowing.Application.Services;

public sealed class UpdateEquipmentService
{
    private readonly IEquipmentRepository _equipment;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateEquipmentService(
        IEquipmentRepository equipment,
        IUnitOfWork unitOfWork)
    {
        _equipment = equipment;
        _unitOfWork = unitOfWork;
    }

    public async Task<EquipmentUpdateResult> UpdateAsync(
        string equipmentId,
        string name,
        string? description,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return new EquipmentUpdateResult
            {
                IsSuccess = false,
                Message = "Equipment name is required."
            };
        }

        var item = await _equipment.GetEquipmentByIdAsync(
            equipmentId.Trim().ToUpperInvariant(),
            cancellationToken);

        if (item is null)
        {
            return new EquipmentUpdateResult
            {
                IsSuccess = false,
                Message = "Equipment was not found."
            };
        }

        item.Name = name.Trim();
        item.Description = string.IsNullOrWhiteSpace(description)
            ? null
            : description.Trim();

        await _equipment.SaveEquipmentAsync(item, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new EquipmentUpdateResult
        {
            IsSuccess = true,
            Message = "Equipment updated."
        };
    }
}