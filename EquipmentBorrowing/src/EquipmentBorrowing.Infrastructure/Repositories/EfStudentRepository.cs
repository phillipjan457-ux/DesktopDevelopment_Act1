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
    public Task AddStudentAsync(
    Student student,
    CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _context.Students.Add(student);
        return Task.CompletedTask;
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
    public Task SaveStudentAsync(
    Student student,
    CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _context.Students.Update(student);
        return Task.CompletedTask;
    }
    public Task DeleteStudentAsync(
    Student student,
    CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _context.Students.Remove(student);
        return Task.CompletedTask;
    }
}