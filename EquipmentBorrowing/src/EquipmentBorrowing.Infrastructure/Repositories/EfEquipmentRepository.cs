using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;
using EquipmentBorrowing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EquipmentBorrowing.Infrastructure.Repositories;

public class EfEquipmentRepository : IEquipmentRepository
{
    private readonly EquipmentBorrowingDbContext _context;

    public EfEquipmentRepository(EquipmentBorrowingDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Equipment>> GetAvailableEquipmentAsync(
    CancellationToken cancellationToken = default)
    {
        return await _context.Equipment
            .AsNoTracking()
            .Where(equipment => !equipment.IsActivelyBorrowed)
            .OrderBy(equipment => equipment.Name)
            .ThenBy(equipment => equipment.EquipmentId)
            .ToListAsync(cancellationToken);
    }
    public async Task<Equipment?> GetEquipmentByIdAsync(
        string equipmentId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Equipment
            .SingleOrDefaultAsync(
                equipment => equipment.EquipmentId == equipmentId,
                cancellationToken);
    }

    public async Task<IEnumerable<Equipment>> GetAllEquipmentAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Equipment
            .AsNoTracking()
            .OrderBy(equipment => equipment.Name)
            .ThenBy(equipment => equipment.EquipmentId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Equipment?> SaveEquipmentAsync(
        Equipment equipment,
        CancellationToken cancellationToken = default)
    {
        var existing = await _context.Equipment
            .SingleOrDefaultAsync(
                item => item.EquipmentId == equipment.EquipmentId,
                cancellationToken);

        if (existing is null)
        {
            _context.Equipment.Add(equipment);
        }
        else
        {
            _context.Entry(existing).CurrentValues.SetValues(equipment);
        }

        // The application operation will commit all changes together.
        return existing ?? equipment;
    }
}