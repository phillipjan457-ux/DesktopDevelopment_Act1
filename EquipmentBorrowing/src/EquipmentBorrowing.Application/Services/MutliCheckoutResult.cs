using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Services;

public sealed class MultiCheckoutResult
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public IReadOnlyList<Borrowing> Borrowings { get; init; }
        = Array.Empty<Borrowing>();
}