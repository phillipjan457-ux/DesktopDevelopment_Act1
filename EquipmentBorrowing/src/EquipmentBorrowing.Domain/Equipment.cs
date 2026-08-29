namespace EquipmentBorrowing.Domain;

public class Equipment
{
    public required string EquipmentId { get; set; }
    public required string Name { get; set; }
    public required string Type { get; set; }
    public bool IsActivelyBorrowed { get; private set; } = false;

    public void MarkAsBorrowed()
    {
        IsActivelyBorrowed = true;
    }

}