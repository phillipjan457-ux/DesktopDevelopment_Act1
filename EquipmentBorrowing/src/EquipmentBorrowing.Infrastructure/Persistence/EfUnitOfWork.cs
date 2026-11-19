using EquipmentBorrowing.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace EquipmentBorrowing.Infrastructure.Persistence;

public class EfUnitOfWork : IUnitOfWork
{
    private readonly EquipmentBorrowingDbContext _context;

    public EfUnitOfWork(EquipmentBorrowingDbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
    public async Task ExecuteInTransactionAsync(
    Func<CancellationToken, Task> operation,
    CancellationToken cancellationToken = default)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                IsolationLevel.Serializable,
                cancellationToken);

        try
        {
            await operation(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
    }
}