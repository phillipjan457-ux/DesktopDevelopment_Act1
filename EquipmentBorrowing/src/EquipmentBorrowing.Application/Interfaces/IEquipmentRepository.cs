namespace EquipmentBorrowing.Application.Interfaces;

using EquipmentBorrowing.Domain;
using System.Threading;
public interface IEquipmentRepository
{
    Task <Equipment?> GetEquipmentByIdAsync(string equipmentId, CancellationToken cancellationToken = default);
    Task <Equipment?> SaveEquipmentAsync(Equipment equipment, CancellationToken cancellationToken = default);
}