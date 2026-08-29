namespace EquipmentBorrowing.Infrastructure.Repositories;

using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

public class InMemoryEquipmentRepository : IEquipmentRepository
{
    private readonly Dictionary<string, Equipment> _equipment;
    public InMemoryEquipmentRepository()
    {
        _equipment = new Dictionary<string, Equipment>
        {
            { "1", new Equipment { EquipmentId = "1", Name = "Laptop", Type = "Electronics"} },
            { "2", new Equipment { EquipmentId = "2", Name = "Projector", Type = "Electronics" } },
            { "3", new Equipment { EquipmentId = "3", Name = "Camera", Type = "Electronics" } }
        };
    }
    public Task<Equipment?> GetEquipmentByIdAsync(string equipmentId, CancellationToken cancellationToken = default)
    {
        _equipment.TryGetValue(equipmentId, out var equipment);
        return Task.FromResult(equipment);
    }
    public Task<Equipment?> SaveEquipmentAsync(Equipment equipment, CancellationToken cancellationToken = default)
    {
        _equipment[equipment.EquipmentId] = equipment;
        return Task.FromResult<Equipment?>(equipment);
    }
}