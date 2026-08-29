using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Infrastructure.Repositories;

var studentRepository = new InMemoryStudentRepository();
var equipmentRepository = new InMemoryEquipmentRepository();
var borrowingRepository = new InMemoryBorrowingRepository();


var service = new BorrowEquipmentService(studentRepository, equipmentRepository, borrowingRepository);
Console.WriteLine("=== Successful Borrow Test ===");
var result = await service.BorrowEquipmentAsync("1", "1");
Console.WriteLine($"Success: {result.IsSuccess}, Message: {result.Message}");

Console.WriteLine();
Console.WriteLine("=== Failure Test: Equipment Not Found ===");
var failResult = await service.BorrowEquipmentAsync("1", "7");
Console.WriteLine($"Success: {failResult.IsSuccess}, Message: {failResult.Message}");