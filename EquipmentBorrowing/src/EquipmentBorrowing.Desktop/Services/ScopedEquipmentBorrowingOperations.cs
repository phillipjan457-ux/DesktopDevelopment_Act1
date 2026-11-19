using EquipmentBorrowing.Application;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Domain;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EquipmentBorrowing.Desktop.Services;

public sealed class ScopedEquipmentBorrowingOperations
    : IEquipmentBorrowingOperations
{
    private readonly IServiceScopeFactory _scopeFactory;

    public ScopedEquipmentBorrowingOperations(
        IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<IEnumerable<Student>> GetStudentsAsync(
        CancellationToken cancellationToken = default)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();

        var repository = scope.ServiceProvider
            .GetRequiredService<IStudentRepository>();

        return await repository.GetAllStudentsAsync(cancellationToken);
    }

    public async Task<IEnumerable<Equipment>> GetEquipmentAsync(
        CancellationToken cancellationToken = default)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();

        var repository = scope.ServiceProvider
            .GetRequiredService<IEquipmentRepository>();

        return await repository.GetAvailableEquipmentAsync(cancellationToken);
    }

    public async Task<IEnumerable<Borrowing>> GetActiveBorrowingsAsync(
        CancellationToken cancellationToken = default)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();

        var repository = scope.ServiceProvider
            .GetRequiredService<IBorrowingRepository>();

        return await repository.ListActiveBorrowings(cancellationToken);
    }

    public async Task<BorrowResult> BorrowAsync(
        string studentId,
        string equipmentId,
        CancellationToken cancellationToken = default)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();

        var service = scope.ServiceProvider
            .GetRequiredService<BorrowEquipmentService>();

        return await service.BorrowEquipmentAsync(
            studentId,
            equipmentId,
            cancellationToken);
    }

    public async Task<ReturnResult> ReturnAsync(
        string borrowId,
        CancellationToken cancellationToken = default)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();

        var service = scope.ServiceProvider
            .GetRequiredService<ReturnEquipmentService>();

        return await service.ReturnEquipmentAsync(
            borrowId,
            cancellationToken);

    }
    public async Task<StudentRegistrationResult> RegisterStudentAsync(
    string studentId,
    string name,
    string? contact,
    CancellationToken cancellationToken = default)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();

        var service = scope.ServiceProvider
            .GetRequiredService<RegisterStudentService>();

        return await service.RegisterAsync(
            studentId,
            name,
            contact,
            cancellationToken);
    }
    public async Task<EquipmentRegistrationResult> RegisterEquipmentAsync(
    string equipmentId,
    string name,
    string? description,
    CancellationToken cancellationToken = default)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();

        var service = scope.ServiceProvider
            .GetRequiredService<RegisterEquipmentService>();

        return await service.RegisterAsync(
            equipmentId,
            name,
            description,
            cancellationToken);
    }
    public async Task<EquipmentUpdateResult> UpdateEquipmentAsync(
    string equipmentId,
    string name,
    string? description,
    CancellationToken cancellationToken = default)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();

        var service = scope.ServiceProvider
            .GetRequiredService<UpdateEquipmentService>();

        return await service.UpdateAsync(
            equipmentId,
            name,
            description,
            cancellationToken);
    }
    public async Task<StudentUpdateResult> UpdateStudentAsync(
    string studentId,
    string newStudentId,
    string name,
    string? program,
    string? contact,
    bool isAuthorized,
    CancellationToken cancellationToken = default)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();

        var service = scope.ServiceProvider
            .GetRequiredService<UpdateStudentService>();

        return await service.UpdateAsync(
    studentId,
    newStudentId,
    name,
    program,
    contact,
    isAuthorized,
    cancellationToken);
    }
    public async Task<MultiCheckoutResult> CheckoutMultipleAsync(
    string studentId,
    IReadOnlyCollection<string> equipmentIds,
    DateOnly expectedReturnDate,
    CancellationToken cancellationToken = default)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();

        var service = scope.ServiceProvider
            .GetRequiredService<CheckoutEquipmentService>();

        return await service.CheckoutAsync(
            studentId,
            equipmentIds,
            expectedReturnDate,
            cancellationToken);
    }
    public async Task<IEnumerable<Borrowing>> GetStudentBorrowingHistoryAsync(
    string studentId,
    CancellationToken cancellationToken = default)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();

        var repository = scope.ServiceProvider
            .GetRequiredService<IBorrowingRepository>();

        return await repository.ListOfBorrows(studentId, cancellationToken);
    }
    public async Task<IEnumerable<Equipment>> GetAllEquipmentAsync(
    CancellationToken cancellationToken = default)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();

        var repository = scope.ServiceProvider
            .GetRequiredService<IEquipmentRepository>();

        return await repository.GetAllEquipmentAsync(cancellationToken);
    }

    public async Task<IEnumerable<Borrowing>> GetEquipmentBorrowingHistoryAsync(
        string equipmentId,
        CancellationToken cancellationToken = default)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();

        var repository = scope.ServiceProvider
            .GetRequiredService<IBorrowingRepository>();

        return await repository.ListByEquipmentAsync(
            equipmentId, cancellationToken);
    }
    public async Task<IEnumerable<Borrowing>> GetAllBorrowingsAsync(
    CancellationToken cancellationToken = default)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();

        var repository = scope.ServiceProvider
            .GetRequiredService<IBorrowingRepository>();

        return await repository.ListAllBorrowings(cancellationToken);
    }
}