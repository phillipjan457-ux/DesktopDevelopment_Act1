using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Services;

public sealed class EquipmentRegistrationResult
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public Equipment? Equipment { get; init; }
}