namespace CareFlow.Domain;

public sealed class Booking
{
    public int BookingId { get; set; }
    public int PatientId { get; set; }
    public required Patient Patient { get; set; }
    public DateOnly BookingDate { get; set; }
    public DateTime BookedAt { get; set; }
    public ICollection<BookingServiceEntry> Services { get; set; } = new List<BookingServiceEntry>();
}
