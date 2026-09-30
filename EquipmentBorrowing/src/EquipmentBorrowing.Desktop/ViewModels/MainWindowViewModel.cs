using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Application.Interfaces;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IEquipmentBorrowingOperations _operations;

    [ObservableProperty]
    private object? currentView;

    public MainWindowViewModel(IEquipmentBorrowingOperations operations)
    {
        _operations = operations;
        CurrentView = "Select a section to begin.";
    }

    [RelayCommand]
    private void ShowEquipment()
    {
        CurrentView = new EquipmentViewModel(_operations);
    }

    [RelayCommand]
    private void ShowBorrowings()
    {
        CurrentView = new BorrowingsViewModel(_operations);
    }
}