namespace EquipmentBorrowing.Domain;

public class Borrowing
{
    public required string BorrowId { get; init; }

    public required string StudentId { get; set; }
    public required Student Student { get; set; }

    public required string EquipmentId { get; set; }
    public required Equipment Equipment { get; set; }

    public required DateTime BorrowDate { get; init; }
    public required DateTime ReturnDate { get; init; }

    public DateTime? ReturnedAt { get; set; }

    public BorrowingStatus Status { get; set; } = BorrowingStatus.Active;

}