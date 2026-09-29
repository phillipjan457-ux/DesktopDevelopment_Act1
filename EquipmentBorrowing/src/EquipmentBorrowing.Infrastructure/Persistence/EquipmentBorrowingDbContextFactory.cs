using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EquipmentBorrowing.Infrastructure.Persistence;

public class EquipmentBorrowingDbContextFactory
    : IDesignTimeDbContextFactory<EquipmentBorrowingDbContext>
{
    public EquipmentBorrowingDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<EquipmentBorrowingDbContext>()
            .UseSqlite("Data Source=equipment-borrowing.db")
            .Options;

        return new EquipmentBorrowingDbContext(options);
    }
}