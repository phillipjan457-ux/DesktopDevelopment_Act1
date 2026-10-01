using EquipmentBorrowing.Application;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Domain;
using Microsoft.Extensions.DependencyInjection;
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
}