namespace CareFlow.Domain;

public sealed class DiagnosticService
{
    public int DiagnosticServiceId { get; set; }
    public required string Name { get; set; }
    public int DailyCapacity { get; set; }
    public ICollection<BookingServiceEntry> BookingServices { get; set; } = new List<BookingServiceEntry>();
}
