using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Services;

public sealed class RegisterStudentService
{
    private readonly IStudentRepository _students;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterStudentService(
        IStudentRepository students,
        IUnitOfWork unitOfWork)
    {
        _students = students;
        _unitOfWork = unitOfWork;
    }

    public async Task<StudentRegistrationResult> RegisterAsync(
        string studentId,
        string name,
        string? contact,
        CancellationToken cancellationToken = default)
    {
        var normalizedId = studentId.Trim().ToUpperInvariant();
        var normalizedName = name.Trim();
        var normalizedContact = string.IsNullOrWhiteSpace(contact)
            ? null
            : contact.Trim();

        if (normalizedId.Length == 0 || normalizedName.Length == 0)
        {
            return new StudentRegistrationResult
            {
                IsSuccess = false,
                Message = "Student ID and name are required."
            };
        }

        if (await _students.GetStudentByIdAsync(
                normalizedId, cancellationToken) is not null)
        {
            return new StudentRegistrationResult
            {
                IsSuccess = false,
                Message = "That student ID is already registered."
            };
        }

        var student = new Student
        {
            StudentId = normalizedId,
            Name = normalizedName,
            Contact = normalizedContact,
            Program = null,
            IsAuthorized = true
        };

        await _students.AddStudentAsync(student, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new StudentRegistrationResult
        {
            IsSuccess = true,
            Message = "Student registered.",
            Student = student
        };
    }
}