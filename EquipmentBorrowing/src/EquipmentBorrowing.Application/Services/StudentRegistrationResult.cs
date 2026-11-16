using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Services;

public sealed class StudentRegistrationResult
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public Student? Student { get; init; }
}