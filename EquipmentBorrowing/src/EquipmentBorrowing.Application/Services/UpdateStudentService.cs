using EquipmentBorrowing.Application.Interfaces;

namespace EquipmentBorrowing.Application.Services;

public sealed class UpdateStudentService
{
    private readonly IStudentRepository _students;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateStudentService(
        IStudentRepository students,
        IUnitOfWork unitOfWork)
    {
        _students = students;
        _unitOfWork = unitOfWork;
    }

    public async Task<StudentUpdateResult> UpdateAsync(
        string studentId,
        string name,
        string? program,
        string? contact,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(studentId) ||
            string.IsNullOrWhiteSpace(name))
        {
            return new StudentUpdateResult
            {
                IsSuccess = false,
                Message = "Student ID and name are required."
            };
        }

        var normalizedId = studentId.Trim().ToUpperInvariant();
        var student = await _students.GetStudentByIdAsync(
            normalizedId,
            cancellationToken);

        if (student is null)
        {
            return new StudentUpdateResult
            {
                IsSuccess = false,
                Message = "Student was not found."
            };
        }

        student.Name = name.Trim();
        student.Program = string.IsNullOrWhiteSpace(program)
            ? null
            : program.Trim();
        student.Contact = string.IsNullOrWhiteSpace(contact)
            ? null
            : contact.Trim();

        await _students.SaveStudentAsync(student, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new StudentUpdateResult
        {
            IsSuccess = true,
            Message = "Student updated."
        };
    }
}