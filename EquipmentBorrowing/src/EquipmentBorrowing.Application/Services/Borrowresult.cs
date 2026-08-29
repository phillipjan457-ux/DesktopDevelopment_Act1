namespace EquipmentBorrowing.Application;
using EquipmentBorrowing.Domain;

public record BorrowResult
{
    public required bool IsSuccess { get; init; } = false;
    public string? Message { get; init; }
    public Borrowing? Borrowing { get; init; }
}