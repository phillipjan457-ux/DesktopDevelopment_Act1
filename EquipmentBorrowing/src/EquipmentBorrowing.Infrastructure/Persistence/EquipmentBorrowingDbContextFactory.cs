using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EquipmentBorrowing.Infrastructure.Persistence;

public class EquipmentBorrowingDbContextFactory
    : IDesignTimeDbContextFactory<EquipmentBorrowingDbContext>
{
    public EquipmentBorrowingDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("EQUIPMENT_BORROWING_CONNECTION")
            ?? throw new InvalidOperationException(
                "Set the EQUIPMENT_BORROWING_CONNECTION environment variable.");

        var options = new DbContextOptionsBuilder<EquipmentBorrowingDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new EquipmentBorrowingDbContext(options);
    }
}