using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Domain;
using EquipmentBorrowing.Infrastructure.Persistence;
using EquipmentBorrowing.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EquipmentBorrowing.Tests;

public class BorrowingPersistenceTests
{
    [Fact]
    public async Task BorrowAndReturn_SaveChangesAcrossContexts()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<EquipmentBorrowingDbContext>()
            .UseSqlite(connection)
            .Options;

        string borrowId;

        // Create the schema, seed it, and borrow the laptop.
        await using (var context = new EquipmentBorrowingDbContext(options))
        {
            await context.Database.MigrateAsync();

            var service = new BorrowEquipmentService(
                new EfStudentRepository(context),
                new EfEquipmentRepository(context),
                new EfBorrowingRepository(context),
                new EfUnitOfWork(context));

            var result = await service.BorrowEquipmentAsync("1", "1");

            Assert.True(result.IsSuccess, result.Message);

            var borrowing = Assert.IsType<Borrowing>(result.Borrowing);
            borrowId = borrowing.BorrowId;
        }

        // A new context must read the saved borrowing from SQLite.
        await using (var context = new EquipmentBorrowingDbContext(options))
        {
            var borrowing = await context.Borrowings
                .SingleAsync(item => item.BorrowId == borrowId);

            var equipment = await context.Equipment
                .SingleAsync(item => item.EquipmentId == "1");

            Assert.Equal(BorrowingStatus.Active, borrowing.Status);
            Assert.Null(borrowing.ReturnedAt);
            Assert.True(equipment.IsActivelyBorrowed);
            Assert.Equal("1", borrowing.StudentId);
            Assert.Equal("1", borrowing.EquipmentId);

            var service = new ReturnEquipmentService(
                new EfBorrowingRepository(context),
                new EfEquipmentRepository(context),
                new EfUnitOfWork(context));

            var result = await service.ReturnEquipmentAsync(borrowId);

            Assert.True(result.IsSuccess, result.Message);
        }

        // Another new context must see the saved return.
        await using (var context = new EquipmentBorrowingDbContext(options))
        {
            var borrowing = await context.Borrowings
                .SingleAsync(item => item.BorrowId == borrowId);

            var equipment = await context.Equipment
                .SingleAsync(item => item.EquipmentId == "1");

            Assert.Equal(BorrowingStatus.Returned, borrowing.Status);
            Assert.NotNull(borrowing.ReturnedAt);
            Assert.False(equipment.IsActivelyBorrowed);
            Assert.Equal(1, await context.Borrowings.CountAsync());
        }
    }
}