namespace EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

public class BorrowEquipmentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IBorrowingRepository _borrowingRepository;
    private const int MaxBorrowLimit = 5;
    public BorrowEquipmentService(IStudentRepository studentRepository, IEquipmentRepository equipmentRepository, IBorrowingRepository borrowingRepository)
    {
        _studentRepository = studentRepository;
        _equipmentRepository = equipmentRepository;
        _borrowingRepository = borrowingRepository;
    }

    public async Task<BorrowResult> BorrowEquipmentAsync(string studentId, string equipmentId, CancellationToken cancellationtoken = default)
    {
        var student = await _studentRepository.GetStudentByIdAsync(studentId, cancellationtoken);

        if (student is null)
        {
            return new BorrowResult
            {
                IsSuccess = false,
                Message = "Student not found."
            };
        }
        if (!student.IsAuthorized)
        {
            return new BorrowResult
            {
                IsSuccess = false,
                Message = "Student is not authorized to borrow equipment."
            };
        }
        var equipment = await _equipmentRepository.GetEquipmentByIdAsync(equipmentId, cancellationtoken);

        if (equipment is null)
        {
            return new BorrowResult
            {
                IsSuccess = false,
                Message = "Equipment not found."
            };
        }
        if (equipment.IsActivelyBorrowed)
        {
            return new BorrowResult
            {
                IsSuccess = false,
                Message = "Equipment is being borrowed."
            };
        }

        var activeBorrows = await _borrowingRepository.ListOfBorrows(studentId, cancellationtoken);
        int activeCount = 0;
        foreach (var borrow in activeBorrows)
        {
            if (borrow.Status == BorrowingStatus.Active)
            {
                activeCount++;
            }
        }

        if (activeCount >= MaxBorrowLimit)
        {
            return new BorrowResult
            {
                IsSuccess = false,
                Message = "Student has reached the maximum borrow limit."
            };
        }

        var borrowing = new Borrowing
        {
            BorrowId = Guid.NewGuid().ToString(),
            Student = student,
            Equipment = equipment,
            BorrowDate = DateTime.UtcNow,
            ReturnDate = DateTime.UtcNow.AddDays(14),
    
        };
        await _borrowingRepository.SaveBorrowing(borrowing, cancellationtoken);
        return new BorrowResult
        {
            IsSuccess = true,
            Message = "Equipment borrowed successfully.",
            Borrowing = borrowing
        };
    }
}  
