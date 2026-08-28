namespace EquipmentBorrowing.Domain;

public class Borrowing
{
	public required string BorrowId { get; init; }
	public required Student Student { get; init; }
	public required Equipment Equipment { get; init; }
	public required DateTime BorrowDate { get; init; }
	public required DateTime ReturnDate { get; init; }
	public BorrowingStatus Status { get; set; } = BorrowingStatus.Active;

}
