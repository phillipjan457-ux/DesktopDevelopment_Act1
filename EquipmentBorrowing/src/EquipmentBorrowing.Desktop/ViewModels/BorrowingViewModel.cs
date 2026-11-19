using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class BorrowingsViewModel : ViewModelBase
{
    private readonly IEquipmentBorrowingOperations _operations;

    [ObservableProperty]
    private ObservableCollection<Borrowing> activeBorrowings = new();

    [ObservableProperty]
    private Borrowing? selectedBorrowing;

    [ObservableProperty]
    private string? statusMessage;

    [ObservableProperty]
    private ObservableCollection<Student> studentList = new();

    [ObservableProperty]
    private Student? selectedHistoryStudent;

    [ObservableProperty]
    private ObservableCollection<Borrowing> studentBorrowingHistory = new();

    [ObservableProperty]
    private string historyStatusMessage = string.Empty;

    [ObservableProperty]
    private ObservableCollection<Equipment> equipmentList = new();

    [ObservableProperty]
    private Equipment? selectedHistoryEquipment;

    [ObservableProperty]
    private ObservableCollection<Borrowing> equipmentBorrowingHistory = new();

    [ObservableProperty]
    private string equipmentHistoryStatusMessage = string.Empty;
    [ObservableProperty]
    private string borrowingSearchText = string.Empty;

    [ObservableProperty]
    private string selectedBorrowingStatusFilter = "All";

    [ObservableProperty]
    private string selectedBorrowingSort = "Newest first";

    [ObservableProperty]
    private ObservableCollection<Borrowing> borrowingSearchResults = new();

    [ObservableProperty]
    private string borrowingSearchStatus = string.Empty;

    public IReadOnlyList<string> BorrowingStatusOptions { get; } =
        new[] { "All", "Active", "Returned" };

    public IReadOnlyList<string> BorrowingSortOptions { get; } =
        new[] { "Newest first", "Oldest first", "Due date (soonest)" };
    public BorrowingsViewModel(IEquipmentBorrowingOperations operations)
    {
        _operations = operations;
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        var equipment = await _operations.GetAllEquipmentAsync();
        EquipmentList = new ObservableCollection<Equipment>(equipment);
        var students = await _operations.GetStudentsAsync();
        StudentList = new ObservableCollection<Student>(students);
        try
        {
            var borrowings = await _operations.GetActiveBorrowingsAsync();

            ActiveBorrowings =
                new ObservableCollection<Borrowing>(borrowings);
        }
        catch (Exception)
        {
            StatusMessage = "Could not load borrowings. Please reopen this section to retry.";
        }
    }

    [RelayCommand]
    private async Task ReturnAsync()
    {
        if (SelectedBorrowing is null)
        {
            StatusMessage = "Please select a borrowing to return.";
            return;
        }

        try
        {
            var result = await _operations.ReturnAsync(
                SelectedBorrowing.BorrowId);

            StatusMessage = result.Message;

            if (result.IsSuccess)
            {
                await LoadDataAsync();
            }
            if (SelectedHistoryStudent is not null)
            {
                await LoadStudentHistoryAsync();
            }
            if (SelectedHistoryEquipment is not null)
            {
                await LoadEquipmentHistoryAsync();
            }
        }
        catch (Exception)
        {
            StatusMessage = "Could not complete the return. Reload the borrowings list before retrying.";
        }
    }
    [RelayCommand]
    private async Task LoadStudentHistoryAsync()
    {
        if (SelectedHistoryStudent is null)
        {
            StudentBorrowingHistory.Clear();
            HistoryStatusMessage = "Select a student to view their borrowing history.";
            return;
        }

        try
        {
            var history = (await _operations.GetStudentBorrowingHistoryAsync(
                SelectedHistoryStudent.StudentId)).ToList();

            StudentBorrowingHistory =
                new ObservableCollection<Borrowing>(history);

            HistoryStatusMessage = history.Count == 0
                ? "This student has no borrowing history."
                : $"Showing {history.Count} borrowing record(s).";
        }
        catch (Exception)
        {
            HistoryStatusMessage = "Could not load this student's borrowing history.";
        }
    }
    [RelayCommand]
    private async Task LoadEquipmentHistoryAsync()
    {
        if (SelectedHistoryEquipment is null)
        {
            EquipmentBorrowingHistory.Clear();
            EquipmentHistoryStatusMessage =
                "Select equipment to view its borrowing history.";
            return;
        }

        try
        {
            var history = (await _operations.GetEquipmentBorrowingHistoryAsync(
                SelectedHistoryEquipment.EquipmentId)).ToList();

            EquipmentBorrowingHistory =
                new ObservableCollection<Borrowing>(history);

            EquipmentHistoryStatusMessage = history.Count == 0
                ? "This equipment has no borrowing history."
                : $"Showing {history.Count} borrowing record(s).";
        }
        catch (Exception)
        {
            EquipmentHistoryStatusMessage =
                "Could not load this equipment's borrowing history.";
        }
    }
    [RelayCommand]
    private async Task SearchBorrowingsAsync()
    {
        try
        {
            var allBorrowings =
                await _operations.GetAllBorrowingsAsync();

            IEnumerable<Borrowing> query = allBorrowings;

            var term = BorrowingSearchText.Trim();

            if (!string.IsNullOrWhiteSpace(term))
            {
                query = query.Where(borrowing =>
                    borrowing.BorrowId.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    borrowing.Student.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    borrowing.Student.StudentId.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    borrowing.Equipment.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    borrowing.Equipment.EquipmentId.Contains(term, StringComparison.OrdinalIgnoreCase));
            }

            if (SelectedBorrowingStatusFilter != "All")
            {
                query = query.Where(borrowing =>
                    borrowing.Status.ToString() == SelectedBorrowingStatusFilter);
            }

            var results = SelectedBorrowingSort switch
            {
                "Oldest first" => query
                    .OrderBy(borrowing => borrowing.BorrowDate)
                    .ThenBy(borrowing => borrowing.BorrowId)
                    .ToList(),

                "Due date (soonest)" => query
                    .OrderBy(borrowing => borrowing.ReturnDate)
                    .ThenBy(borrowing => borrowing.BorrowId)
                    .ToList(),

                _ => query
                    .OrderByDescending(borrowing => borrowing.BorrowDate)
                    .ThenBy(borrowing => borrowing.BorrowId)
                    .ToList()
            };

            BorrowingSearchResults =
                new ObservableCollection<Borrowing>(results);

            BorrowingSearchStatus =
                $"Showing {results.Count} borrowing record(s).";
        }
        catch (Exception)
        {
            BorrowingSearchStatus = "Could not search borrowing records.";
        }
    }
}