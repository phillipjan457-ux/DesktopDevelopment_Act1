using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

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

    public BorrowingsViewModel(IEquipmentBorrowingOperations operations)
    {
        _operations = operations;
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
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
        }
        catch (Exception)
        {
            StatusMessage = "Could not complete the return. Reload the borrowings list before retrying.";
        }
    }
}