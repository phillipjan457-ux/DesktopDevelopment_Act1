namespace EquipmentBorrowing.Domain;

public class Equipment
{
    public required string EquipmentId { get; set; }
    public required string Name { get; set; }

    // Kept for compatibility with the earlier activity, but it is optional.
    public string? Type { get; set; }

    // Activity 4 allows a description to be omitted.
    public string? Description { get; set; }

    public bool IsActivelyBorrowed { get; private set; } = false;

    public void MarkAsBorrowed()
    {
        IsActivelyBorrowed = true;
    }

    public void MarkAsAvailable()
    {
        IsActivelyBorrowed = false;
    }
}