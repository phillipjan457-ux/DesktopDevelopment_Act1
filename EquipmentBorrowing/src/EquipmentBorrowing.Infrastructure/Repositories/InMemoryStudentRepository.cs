namespace EquipmentBorrowing.Infrastructure.Repositories;

using EquipmentBorrowing.Domain;
using EquipmentBorrowing.Application.Interfaces;

public class InMemoryStudentRepository : IStudentRepository
{
    private readonly Dictionary<string, Student> _students;
    public InMemoryStudentRepository()
    {
        _students = new Dictionary<string, Student>
        {
            { "1", new Student { StudentId = "1", Name = "John Doe", Program = "Computer Science", IsAuthorized = true } },
            { "2", new Student { StudentId = "2", Name = "Jane Smith", Program = "Mathematics", IsAuthorized = false } },
            { "3", new Student { StudentId = "3", Name = "Alice Johnson", Program = "Physics", IsAuthorized = true } }
        };
    }
    public Task<Student?> GetStudentByIdAsync(string studentId, CancellationToken cancellationToken = default)
    {
        _students.TryGetValue(studentId, out var student);
        return Task.FromResult(student);
    }
}