using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class EquipmentViewModel : ViewModelBase
{
    private readonly IEquipmentBorrowingOperations _operations;

    [ObservableProperty]
    private ObservableCollection<Equipment> equipmentList = new();

    [ObservableProperty]
    private ObservableCollection<Student> studentList = new();

    [ObservableProperty]
    private Equipment? selectedEquipment;

    [ObservableProperty]
    private Student? selectedStudent;

    [ObservableProperty]
    private string? statusMessage;
    [ObservableProperty]
    private string studentIdInput = string.Empty;

    [ObservableProperty]
    private string studentNameInput = string.Empty;

    [ObservableProperty]
    private string studentContactInput = string.Empty;

    public EquipmentViewModel(IEquipmentBorrowingOperations operations)
    {
        _operations = operations;
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            var equipment = await _operations.GetEquipmentAsync();
            var students = await _operations.GetStudentsAsync();

            EquipmentList = new ObservableCollection<Equipment>(equipment);
            StudentList = new ObservableCollection<Student>(students);
        }
        catch (Exception)
        {
            StatusMessage = "Could not load equipment and students. Please reopen this section to retry.";
        }
    }
    [RelayCommand]
    private async Task RegisterStudentAsync()
    {
        if (string.IsNullOrWhiteSpace(StudentIdInput) ||
            string.IsNullOrWhiteSpace(StudentNameInput))
        {
            StatusMessage = "Student ID and name are required.";
            return;
        }

        try
        {
            var result = await _operations.RegisterStudentAsync(
                StudentIdInput,
                StudentNameInput,
                StudentContactInput);

            StatusMessage = result.Message;

            if (result.IsSuccess)
            {
                StudentIdInput = string.Empty;
                StudentNameInput = string.Empty;
                StudentContactInput = string.Empty;
                await LoadDataAsync();
            }
        }
        catch (Exception)
        {
            StatusMessage =
                "Could not register the student. Check the ID and try again.";
        }
    }

    [RelayCommand]
    private async Task BorrowAsync()
    {
        if (SelectedStudent is null)
        {
            StatusMessage = "Please select a student.";
            return;
        }

        if (SelectedEquipment is null)
        {
            StatusMessage = "Please select equipment to borrow.";
            return;
        }

        try
        {
            var result = await _operations.BorrowAsync(
                SelectedStudent.StudentId,
                SelectedEquipment.EquipmentId);

            StatusMessage = result.Message;

            if (result.IsSuccess)
            {
                await LoadDataAsync();
            }
        }
        catch (Exception)
        {
            StatusMessage = "Could not complete the borrowing. Reload the equipment list before retrying.";
        }
    }
}