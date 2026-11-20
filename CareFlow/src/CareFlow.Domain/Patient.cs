namespace CareFlow.Domain;

public sealed class Patient
{
    public int PatientId { get; set; }
    public required string FullName { get; set; }
}
