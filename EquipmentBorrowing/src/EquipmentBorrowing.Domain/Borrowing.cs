namespace EquipmentBorrowing.Domain;

public class Borrowing
{
    public required string BorrowId { get; init; }

    public required string StudentId { get; init; }
    public required Student Student { get; init; }

    public required string EquipmentId { get; init; }
    public required Equipment Equipment { get; init; }

    public required DateTime BorrowDate { get; init; }
    public required DateTime ReturnDate { get; init; }

    public DateTime? ReturnedAt { get; set; }

    public BorrowingStatus Status { get; set; } = BorrowingStatus.Active;
}