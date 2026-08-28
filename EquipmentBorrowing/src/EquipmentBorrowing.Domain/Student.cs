namespace EquipmentBorrowing.Domain;

public class Student
{
    public required string StudentId { get; set; }
    public required string Name { get; set; }
    public required string Program { get; set; }
    public bool IsAuthorized { get; init; } = false;
}