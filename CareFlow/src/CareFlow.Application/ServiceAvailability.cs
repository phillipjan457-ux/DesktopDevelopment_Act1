namespace CareFlow.Application;

public sealed record ServiceAvailability(int DiagnosticServiceId, string Name, int DailyCapacity, int BookedCount)
{
    public int Remaining => Math.Max(0, DailyCapacity - BookedCount);
}
