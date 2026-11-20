using CareFlow.Domain;
using Microsoft.EntityFrameworkCore;

namespace CareFlow.Infrastructure;

public sealed class CareFlowDbContext(DbContextOptions<CareFlowDbContext> options) : DbContext(options)
{
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<DiagnosticService> DiagnosticServices => Set<DiagnosticService>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingServiceEntry> BookingServices => Set<BookingServiceEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Patient>(e =>
        {
            e.HasKey(x => x.PatientId);
            e.Property(x => x.FullName).HasMaxLength(120).IsRequired();
        });
        modelBuilder.Entity<DiagnosticService>(e =>
        {
            e.HasKey(x => x.DiagnosticServiceId);
            e.Property(x => x.Name).HasMaxLength(80).IsRequired();
            e.HasIndex(x => x.Name).IsUnique();
            e.ToTable(t => t.HasCheckConstraint("CK_DiagnosticServices_Capacity", "DailyCapacity > 0"));
        });
        modelBuilder.Entity<Booking>(e =>
        {
            e.HasKey(x => x.BookingId);
            e.HasOne(x => x.Patient).WithMany().HasForeignKey(x => x.PatientId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => new { x.BookingDate, x.PatientId });
        });
        modelBuilder.Entity<BookingServiceEntry>(e =>
        {
            e.HasKey(x => new { x.BookingId, x.DiagnosticServiceId });
            e.HasOne(x => x.Booking).WithMany(x => x.Services).HasForeignKey(x => x.BookingId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.DiagnosticService).WithMany(x => x.BookingServices).HasForeignKey(x => x.DiagnosticServiceId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => x.DiagnosticServiceId);
        });
    }
}
