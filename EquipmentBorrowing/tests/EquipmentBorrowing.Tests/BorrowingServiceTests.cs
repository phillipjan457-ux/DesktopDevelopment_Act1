using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Domain;
using EquipmentBorrowing.Infrastructure.Repositories;

namespace EquipmentBorrowing.Tests;

public class BorrowingServiceTests
{
    [Fact]
    public async Task BorrowAsync_ValidRequest_CreatesActiveBorrowing()
    {
        var fixture = new TestFixture();
        var service = fixture.CreateBorrowService();

        var result = await service.BorrowEquipmentAsync("1", "1");

        Assert.True(result.IsSuccess, result.Message);
        var borrowing = Assert.IsType<Borrowing>(result.Borrowing);
        Assert.Equal(BorrowingStatus.Active, borrowing.Status);
        Assert.True((await fixture.Equipment.GetEquipmentByIdAsync("1"))!
            .IsActivelyBorrowed);
    }

    [Fact]
    public async Task BorrowAsync_UnknownStudent_IsRejected()
    {
        var fixture = new TestFixture();

        var result = await fixture.CreateBorrowService()
            .BorrowEquipmentAsync("missing", "1");

        Assert.False(result.IsSuccess);
        Assert.Equal("Student not found.", result.Message);
    }

    [Fact]
    public async Task BorrowAsync_UnauthorizedStudent_IsRejected()
    {
        var fixture = new TestFixture();

        var result = await fixture.CreateBorrowService()
            .BorrowEquipmentAsync("2", "1");

        Assert.False(result.IsSuccess);
        Assert.Equal("Student is not authorized to borrow equipment.", result.Message);
    }

    [Fact]
    public async Task BorrowAsync_UnknownEquipment_IsRejected()
    {
        var fixture = new TestFixture();

        var result = await fixture.CreateBorrowService()
            .BorrowEquipmentAsync("1", "missing");

        Assert.False(result.IsSuccess);
        Assert.Equal("Equipment not found.", result.Message);
    }

    [Fact]
    public async Task BorrowAsync_AlreadyBorrowedEquipment_IsRejected()
    {
        var fixture = new TestFixture();
        (await fixture.Equipment.GetEquipmentByIdAsync("1"))!.MarkAsBorrowed();

        var result = await fixture.CreateBorrowService()
            .BorrowEquipmentAsync("1", "1");

        Assert.False(result.IsSuccess);
        Assert.Equal("Equipment is being borrowed.", result.Message);
    }

    [Fact]
    public async Task BorrowAsync_StudentAtBorrowLimit_IsRejected()
    {
        var fixture = new TestFixture();
        await fixture.AddEquipmentAsync("4");
        await fixture.AddActiveBorrowingAsync("1");
        await fixture.AddActiveBorrowingAsync("2");
        await fixture.AddActiveBorrowingAsync("3");

        var result = await fixture.CreateBorrowService()
            .BorrowEquipmentAsync("1", "4");

        Assert.False(result.IsSuccess);
        Assert.Equal("Student has reached the maximum borrow limit.", result.Message);
    }

    [Fact]
    public async Task ReturnAsync_ActiveBorrowing_MarksItReturnedAndMakesEquipmentAvailable()
    {
        var fixture = new TestFixture();
        var borrowing = await fixture.AddActiveBorrowingAsync("1");
        borrowing.Equipment.MarkAsBorrowed();

        var result = await fixture.CreateReturnService()
            .ReturnEquipmentAsync(borrowing.BorrowId);

        Assert.True(result.IsSuccess, result.Message);
        Assert.Equal(BorrowingStatus.Returned, borrowing.Status);
        Assert.NotNull(borrowing.ReturnedAt);
        Assert.False(borrowing.Equipment.IsActivelyBorrowed);
    }

    [Fact]
    public async Task ReturnAsync_UnknownBorrowing_IsRejected()
    {
        var fixture = new TestFixture();

        var result = await fixture.CreateReturnService()
            .ReturnEquipmentAsync("missing");

        Assert.False(result.IsSuccess);
        Assert.Equal("Borrowing record not found.", result.Message);
    }

    [Fact]
    public async Task ReturnAsync_AlreadyReturnedBorrowing_IsRejected()
    {
        var fixture = new TestFixture();
        var borrowing = await fixture.AddActiveBorrowingAsync("1");
        borrowing.Status = BorrowingStatus.Returned;

        var result = await fixture.CreateReturnService()
            .ReturnEquipmentAsync(borrowing.BorrowId);

        Assert.False(result.IsSuccess);
        Assert.Equal("This equipment has already been returned.", result.Message);
    }

    [Fact]
    public async Task CheckoutAsync_TwoAvailableItems_CreatesTwoActiveBorrowings()
    {
        var fixture = new TestFixture();
        var dueDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(2);

        var result = await fixture.CreateCheckoutService()
            .CheckoutAsync("1", new[] { "1", "2" }, dueDate);

        Assert.True(result.IsSuccess, result.Message);
        Assert.Equal(2, result.Borrowings.Count);
        Assert.All(result.Borrowings, borrowing =>
            Assert.Equal(BorrowingStatus.Active, borrowing.Status));
        Assert.True((await fixture.Equipment.GetEquipmentByIdAsync("1"))!
            .IsActivelyBorrowed);
        Assert.True((await fixture.Equipment.GetEquipmentByIdAsync("2"))!
            .IsActivelyBorrowed);
    }

    [Fact]
    public async Task CheckoutAsync_FewerThanTwoItems_IsRejected()
    {
        var fixture = new TestFixture();
        var dueDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(2);

        var result = await fixture.CreateCheckoutService()
            .CheckoutAsync("1", new[] { "1" }, dueDate);

        Assert.False(result.IsSuccess);
        Assert.Equal("Select at least two equipment items.", result.Message);
    }

    [Fact]
    public async Task CheckoutAsync_DuplicateEquipmentIds_IsRejected()
    {
        var fixture = new TestFixture();
        var dueDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(2);

        var result = await fixture.CreateCheckoutService()
            .CheckoutAsync("1", new[] { "1", "1" }, dueDate);

        Assert.False(result.IsSuccess);
        Assert.Equal("The same equipment item cannot be selected twice.", result.Message);
    }

    [Fact]
    public async Task CheckoutAsync_UnauthorizedStudent_IsRejected()
    {
        var fixture = new TestFixture();
        var dueDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(2);

        var result = await fixture.CreateCheckoutService()
            .CheckoutAsync("2", new[] { "1", "3" }, dueDate);

        Assert.False(result.IsSuccess);
        Assert.Equal("Student is not authorized to borrow equipment.", result.Message);
    }

    [Fact]
    public async Task CheckoutAsync_WouldExceedBorrowLimit_IsRejected()
    {
        var fixture = new TestFixture();
        await fixture.AddActiveBorrowingAsync("2");
        await fixture.AddActiveBorrowingAsync("3");
        var dueDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(2);

        var result = await fixture.CreateCheckoutService()
            .CheckoutAsync("1", new[] { "1", "2" }, dueDate);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "The student would exceed the limit of three active items.",
            result.Message);
    }

    private sealed class TestFixture
    {
        public InMemoryStudentRepository Students { get; } = new();
        public InMemoryEquipmentRepository Equipment { get; } = new();
        public InMemoryBorrowingRepository Borrowings { get; } = new();
        private InMemoryUnitOfWork UnitOfWork { get; } = new();

        public BorrowEquipmentService CreateBorrowService() =>
            new(Students, Equipment, Borrowings, UnitOfWork);

        public ReturnEquipmentService CreateReturnService() =>
            new(Borrowings, Equipment, UnitOfWork);

        public CheckoutEquipmentService CreateCheckoutService() =>
            new(Students, Equipment, Borrowings, UnitOfWork);

        public async Task AddEquipmentAsync(string equipmentId)
        {
            await Equipment.AddEquipmentAsync(new Equipment
            {
                EquipmentId = equipmentId,
                Name = $"Equipment {equipmentId}"
            });
        }

        public async Task<Borrowing> AddActiveBorrowingAsync(
            string equipmentId,
            string studentId = "1")
        {
            var student = (await Students.GetStudentByIdAsync(studentId))!;
            var equipment = (await Equipment.GetEquipmentByIdAsync(equipmentId))!;
            var now = DateTime.UtcNow;

            var borrowing = new Borrowing
            {
                BorrowId = Guid.NewGuid().ToString(),
                StudentId = student.StudentId,
                Student = student,
                EquipmentId = equipment.EquipmentId,
                Equipment = equipment,
                BorrowDate = now,
                ReturnDate = now.AddDays(14),
                Status = BorrowingStatus.Active
            };

            await Borrowings.SaveBorrowing(borrowing);
            return borrowing;
        }
    }
}