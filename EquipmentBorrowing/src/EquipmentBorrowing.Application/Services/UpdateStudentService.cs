using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Services;

public sealed class UpdateStudentService
{
    private readonly IStudentRepository _students;
    private readonly IBorrowingRepository _borrowings;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateStudentService(
        IStudentRepository students,
        IBorrowingRepository borrowings,
        IUnitOfWork unitOfWork)
    {
        _students = students;
        _borrowings = borrowings;
        _unitOfWork = unitOfWork;
    }

    public async Task<StudentUpdateResult> UpdateAsync(
        string studentId,
        string newStudentId,
        string name,
        string? program,
        string? contact,
        bool isAuthorized,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(studentId) ||
            string.IsNullOrWhiteSpace(newStudentId) ||
            string.IsNullOrWhiteSpace(name))
        {
            return Rejected("Student ID and name are required.");
        }

        var oldId = studentId.Trim().ToUpperInvariant();
        var newId = newStudentId.Trim().ToUpperInvariant();
        StudentUpdateResult? result = null;

        await _unitOfWork.ExecuteInTransactionAsync(async token =>
        {
            var current = await _students.GetStudentByIdAsync(oldId, token);

            if (current is null)
            {
                result = Rejected("Student was not found.");
                return;
            }

            if (newId != oldId &&
                await _students.GetStudentByIdAsync(newId, token) is not null)
            {
                result = Rejected("That institutional number is already in use.");
                return;
            }

            if (newId == oldId)
            {
                current.Name = name.Trim();
                current.Program = NormalizeOptional(program);
                current.Contact = NormalizeOptional(contact);
                current.IsAuthorized = isAuthorized;

                await _students.SaveStudentAsync(current, token);
            }
            else
            {
                var replacement = new Student
                {
                    StudentId = newId,
                    Name = name.Trim(),
                    Program = NormalizeOptional(program),
                    Contact = NormalizeOptional(contact),
                    IsAuthorized = isAuthorized
                };

                // Insert the new student first so borrowing records can reference it.
                await _students.AddStudentAsync(replacement, token);
                await _unitOfWork.SaveChangesAsync(token);

                await _borrowings.UpdateStudentIdAsync(oldId, replacement, token);
                await _students.DeleteStudentAsync(current, token);
            }

            await _unitOfWork.SaveChangesAsync(token);
            result = new StudentUpdateResult
            {
                IsSuccess = true,
                Message = "Student updated."
            };
        }, cancellationToken);

        return result ?? Rejected("Student update did not complete.");
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static StudentUpdateResult Rejected(string message) =>
        new()
        {
            IsSuccess = false,
            Message = message
        };
}