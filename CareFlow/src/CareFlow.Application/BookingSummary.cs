namespace CareFlow.Application;

public sealed record BookingSummary(int BookingId, string PatientName, DateOnly Date, IReadOnlyList<string> Services)
{
    public string ServicesText => string.Join(", ", Services);
}
