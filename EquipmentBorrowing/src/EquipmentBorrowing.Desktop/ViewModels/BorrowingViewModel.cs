using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Domain;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class BorrowingsViewModel : ViewModelBase
{
    private readonly IBorrowingRepository _borrowingRepository;
    private readonly ReturnEquipmentService _returnEquipmentService;

    [ObservableProperty]
    private ObservableCollection<Borrowing> activeBorrowings = new();

    [ObservableProperty]
    private Borrowing? selectedBorrowing;

    [ObservableProperty]
    private string? statusMessage;

    public BorrowingsViewModel(
        IBorrowingRepository borrowingRepository,
        ReturnEquipmentService returnEquipmentService)
    {
        _borrowingRepository = borrowingRepository;
        _returnEquipmentService = returnEquipmentService;
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        var borrowings = await _borrowingRepository.ListActiveBorrowings();
        ActiveBorrowings = new ObservableCollection<Borrowing>(borrowings);
    }

    [RelayCommand]
    private async Task ReturnAsync()
    {
        if (SelectedBorrowing is null)
        {
            StatusMessage = "Please select a borrowing to return.";
            return;
        }

        var result = await _returnEquipmentService.ReturnEquipmentAsync(SelectedBorrowing.BorrowId);

        StatusMessage = result.Message;

        if (result.IsSuccess)
        {
            await LoadDataAsync();
        }
    }
}