namespace CareFlow.Application;

public sealed class BookingService(ICareFlowRepository repository)
{
    public Task<IReadOnlyList<ServiceAvailability>> GetAvailabilityAsync(DateOnly date, CancellationToken token = default) =>
        repository.GetAvailabilityAsync(date, token);

    public Task<IReadOnlyList<BookingSummary>> GetBookingsAsync(CancellationToken token = default) =>
        repository.GetBookingsAsync(token);

    public Task<IReadOnlyList<CareFlow.Domain.Patient>> GetPatientsAsync(CancellationToken token = default) =>
        repository.GetPatientsAsync(token);

    public async Task<int> ConfirmAsync(int? patientId, DateOnly date, IReadOnlyList<int> serviceIds, CancellationToken token = default)
    {
        if (patientId is null or <= 0) throw new InvalidOperationException("Select a patient.");
        if (date < DateOnly.FromDateTime(DateTime.Now)) throw new InvalidOperationException("The booking date cannot be earlier than today.");
        if (serviceIds.Count is < 1 or > 3) throw new InvalidOperationException("Select between one and three diagnostic services.");
        if (serviceIds.Distinct().Count() != serviceIds.Count) throw new InvalidOperationException("A service can only be selected once.");
        return await repository.ConfirmBookingAsync(patientId.Value, date, serviceIds, token);
    }
}
