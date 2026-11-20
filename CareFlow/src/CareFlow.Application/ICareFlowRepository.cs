using CareFlow.Domain;

namespace CareFlow.Application;

public interface ICareFlowRepository
{
    Task<IReadOnlyList<Patient>> GetPatientsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ServiceAvailability>> GetAvailabilityAsync(DateOnly date, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BookingSummary>> GetBookingsAsync(CancellationToken cancellationToken = default);
    Task<int> ConfirmBookingAsync(int patientId, DateOnly date, IReadOnlyList<int> serviceIds, CancellationToken cancellationToken = default);
}
