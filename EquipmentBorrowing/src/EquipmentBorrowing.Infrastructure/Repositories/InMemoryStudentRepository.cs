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
    public Task<IEnumerable<Student>> GetAllStudentsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_students.Values.AsEnumerable());
    }
    public Task AddStudentAsync(
    Student student,
    CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_students.ContainsKey(student.StudentId))
        {
            throw new InvalidOperationException("Student ID already exists.");
        }

        _students.Add(student.StudentId, student);
        return Task.CompletedTask;
    }
    public Task SaveStudentAsync(
    Student student,
    CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _students[student.StudentId] = student;
        return Task.CompletedTask;
    }
}