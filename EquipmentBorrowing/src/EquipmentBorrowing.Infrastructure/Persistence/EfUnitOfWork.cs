using EquipmentBorrowing.Application.Interfaces;

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
}