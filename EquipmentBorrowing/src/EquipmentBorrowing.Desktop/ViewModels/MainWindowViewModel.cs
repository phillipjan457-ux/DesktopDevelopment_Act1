using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private object? currentView;

    public MainWindowViewModel()
    {
        CurrentView = "Select a section to begin.";
    }

    [RelayCommand]
    private void ShowEquipment()
    {
        CurrentView = "Equipment view placeholder — built in Part D.";
    }

    [RelayCommand]
    private void ShowBorrowings()
    {
        CurrentView = "Active Borrowings view placeholder — built in Part F.";
    }
}