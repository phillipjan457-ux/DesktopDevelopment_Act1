namespace EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;
using System.Threading;
public interface IStudentRepository
{
    Task<Student?> GetStudentByIdAsync(string studentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Student>> GetAllStudentsAsync(CancellationToken cancellationToken = default);

}