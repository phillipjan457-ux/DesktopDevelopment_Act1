namespace CareFlow.Domain;

public sealed class BookingServiceEntry
{
    public int BookingId { get; set; }
    public required Booking Booking { get; set; }
    public int DiagnosticServiceId { get; set; }
    public required DiagnosticService DiagnosticService { get; set; }
}
