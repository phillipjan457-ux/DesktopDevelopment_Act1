using CareFlow.Application;
using CareFlow.Domain;
using Microsoft.EntityFrameworkCore;

namespace CareFlow.Infrastructure;

public sealed class CareFlowRepository(CareFlowDbContext db) : ICareFlowRepository
{
    public async Task<IReadOnlyList<Patient>> GetPatientsAsync(CancellationToken token = default) =>
        await db.Patients.AsNoTracking().OrderBy(p => p.FullName).ToListAsync(token);

    public async Task<IReadOnlyList<ServiceAvailability>> GetAvailabilityAsync(DateOnly date, CancellationToken token = default)
    {
        var counts = await db.BookingServices.AsNoTracking()
            .Where(x => x.Booking.BookingDate == date)
            .GroupBy(x => x.DiagnosticServiceId)
            .Select(g => new { Id = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Id, x => x.Count, token);
        var services = await db.DiagnosticServices.AsNoTracking().OrderBy(s => s.Name).ToListAsync(token);
        return services.Select(s => new ServiceAvailability(s.DiagnosticServiceId, s.Name, s.DailyCapacity,
            counts.GetValueOrDefault(s.DiagnosticServiceId))).ToList();
    }

    public async Task<IReadOnlyList<BookingSummary>> GetBookingsAsync(CancellationToken token = default)
    {
        var bookings = await db.Bookings.AsNoTracking().Include(b => b.Patient)
            .Include(b => b.Services).ThenInclude(s => s.DiagnosticService)
            .OrderByDescending(b => b.BookingDate).ThenByDescending(b => b.BookingId).ToListAsync(token);
        return bookings.Select(b => new BookingSummary(b.BookingId, b.Patient.FullName, b.BookingDate,
            b.Services.Select(s => s.DiagnosticService.Name).OrderBy(n => n).ToList())).ToList();
    }

    public async Task<int> ConfirmBookingAsync(int patientId, DateOnly date, IReadOnlyList<int> serviceIds, CancellationToken token = default)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(token);
        try
        {
            var patient = await db.Patients.SingleOrDefaultAsync(p => p.PatientId == patientId, token)
                ?? throw new InvalidOperationException("The selected patient was not found.");
            var selected = await db.DiagnosticServices.Where(s => serviceIds.Contains(s.DiagnosticServiceId)).ToListAsync(token);
            if (selected.Count != serviceIds.Count) throw new InvalidOperationException("One or more selected services no longer exist.");
            var counts = await db.BookingServices.Where(x => serviceIds.Contains(x.DiagnosticServiceId) && x.Booking.BookingDate == date)
                .GroupBy(x => x.DiagnosticServiceId).Select(g => new { Id = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Id, x => x.Count, token);
            var full = selected.FirstOrDefault(s => counts.GetValueOrDefault(s.DiagnosticServiceId) >= s.DailyCapacity);
            if (full is not null) throw new InvalidOperationException($"{full.Name} has reached its daily capacity. Refresh availability and choose again.");

            var booking = new Booking { PatientId = patientId, Patient = patient, BookingDate = date, BookedAt = DateTime.Now };
            foreach (var service in selected)
                booking.Services.Add(new BookingServiceEntry { Booking = booking, DiagnosticService = service, DiagnosticServiceId = service.DiagnosticServiceId });
            db.Bookings.Add(booking);
            await db.SaveChangesAsync(token);
            await transaction.CommitAsync(token);
            return booking.BookingId;
        }
        catch
        {
            await transaction.RollbackAsync(token);
            throw;
        }
    }
}

public static class CareFlowSeed
{
    public static async Task InitializeAsync(CareFlowDbContext db, CancellationToken token = default)
    {
        if (!await db.Patients.AnyAsync(token))
            db.Patients.AddRange(new Patient { FullName = "Alex Morgan" }, new Patient { FullName = "Jamie Rivera" }, new Patient { FullName = "Taylor Santos" });
        if (!await db.DiagnosticServices.AnyAsync(token))
            db.DiagnosticServices.AddRange(
                new DiagnosticService { Name = "Complete Blood Count", DailyCapacity = 20 },
                new DiagnosticService { Name = "Urinalysis", DailyCapacity = 18 },
                new DiagnosticService { Name = "Chest X-Ray", DailyCapacity = 10 },
                new DiagnosticService { Name = "ECG", DailyCapacity = 12 });
        await db.SaveChangesAsync(token);
    }
}
