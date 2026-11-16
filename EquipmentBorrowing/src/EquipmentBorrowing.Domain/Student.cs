namespace EquipmentBorrowing.Domain;

public class Student
{
    public required string StudentId { get; set; }
    public required string Name { get; set; }

    // Kept for compatibility with the earlier activity, but it is optional.
    public string? Program { get; set; }

    // Activity 4 allows contact information to be omitted.
    public string? Contact { get; set; }

    // New students are eligible to borrow by default.
    // Keep this property name for now because the existing service uses it.
    public bool IsAuthorized { get; set; } = true;
}