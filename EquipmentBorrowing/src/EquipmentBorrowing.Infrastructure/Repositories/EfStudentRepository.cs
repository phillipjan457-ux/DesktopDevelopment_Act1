using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;
using EquipmentBorrowing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EquipmentBorrowing.Infrastructure.Repositories;

public class EfStudentRepository : IStudentRepository
{
    private readonly EquipmentBorrowingDbContext _context;

    public EfStudentRepository(EquipmentBorrowingDbContext context)
    {
        _context = context;
    }

    public async Task<Student?> GetStudentByIdAsync(
        string studentId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Students
            .SingleOrDefaultAsync(
                student => student.StudentId == studentId,
                cancellationToken);
    }

    public async Task<IEnumerable<Student>> GetAllStudentsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Students
            .AsNoTracking()
            .OrderBy(student => student.Name)
            .ThenBy(student => student.StudentId)
            .ToListAsync(cancellationToken);
    }
}