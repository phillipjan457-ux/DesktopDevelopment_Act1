using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Application.Services;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly BorrowEquipmentService _borrowEquipmentService;

    [ObservableProperty]
    private object? currentView;

    public MainWindowViewModel(
        IEquipmentRepository equipmentRepository,
        IStudentRepository studentRepository,
        BorrowEquipmentService borrowEquipmentService)
    {
        _equipmentRepository = equipmentRepository;
        _studentRepository = studentRepository;
        _borrowEquipmentService = borrowEquipmentService;
        CurrentView = "Select a section to begin.";
    }

    [RelayCommand]
    private void ShowEquipment()
    {
        CurrentView = new EquipmentViewModel(_equipmentRepository, _studentRepository, _borrowEquipmentService);
    }

    [RelayCommand]
    private void ShowBorrowings()
    {
        CurrentView = "Active Borrowings view placeholder — built in Part F.";
    }
}