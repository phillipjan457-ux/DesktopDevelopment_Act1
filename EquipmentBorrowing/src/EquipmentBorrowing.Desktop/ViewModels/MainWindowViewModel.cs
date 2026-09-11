using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Application.Services;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IBorrowingRepository _borrowingRepository;
    private readonly BorrowEquipmentService _borrowEquipmentService;
    private readonly ReturnEquipmentService _returnEquipmentService;

    [ObservableProperty]
    private object? currentView;

    public MainWindowViewModel(
        IEquipmentRepository equipmentRepository,
        IStudentRepository studentRepository,
        IBorrowingRepository borrowingRepository,
        BorrowEquipmentService borrowEquipmentService,
        ReturnEquipmentService returnEquipmentService)
    {
        _equipmentRepository = equipmentRepository;
        _studentRepository = studentRepository;
        _borrowingRepository = borrowingRepository;
        _borrowEquipmentService = borrowEquipmentService;
        _returnEquipmentService = returnEquipmentService;
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
        CurrentView = new BorrowingsViewModel(_borrowingRepository, _returnEquipmentService);
    }
}