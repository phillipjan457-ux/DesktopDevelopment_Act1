namespace EquipmentBorrowing.Application.Services;

public sealed class StudentUpdateResult
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
}