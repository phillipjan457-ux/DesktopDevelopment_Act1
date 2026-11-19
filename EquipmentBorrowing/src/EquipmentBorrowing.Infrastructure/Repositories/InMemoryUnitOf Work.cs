using EquipmentBorrowing.Application.Interfaces;

namespace EquipmentBorrowing.Infrastructure.Repositories;

public class InMemoryUnitOfWork : IUnitOfWork
{
    public Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // In-memory repositories already update their collections directly.
        return Task.FromResult(0);
    }
    public Task ExecuteInTransactionAsync(
    Func<CancellationToken, Task> operation,
    CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return operation(cancellationToken);
    }
}