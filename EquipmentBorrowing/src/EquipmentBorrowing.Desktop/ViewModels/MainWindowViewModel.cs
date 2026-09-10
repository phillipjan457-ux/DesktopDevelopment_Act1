using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Application.Interfaces;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IEquipmentRepository _equipmentRepository;

    [ObservableProperty]
    private object? currentView;

    public MainWindowViewModel(IEquipmentRepository equipmentRepository)
    {
        _equipmentRepository = equipmentRepository;
        CurrentView = "Select a section to begin.";
    }

    [RelayCommand]
    private void ShowEquipment()
    {
        CurrentView = new EquipmentViewModel(_equipmentRepository);
    }

    [RelayCommand]
    private void ShowBorrowings()
    {
        CurrentView = "Active Borrowings view placeholder — built in Part F.";
    }
}