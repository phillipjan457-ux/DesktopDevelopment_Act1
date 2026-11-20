using CommunityToolkit.Mvvm.ComponentModel;
using CareFlow.Application;

namespace CareFlow.Desktop.ViewModels;

public partial class ServiceChoice(ServiceAvailability availability) : ObservableObject
{
    public int Id => availability.DiagnosticServiceId;
    public string Name => availability.Name;
    public int Remaining => availability.Remaining;
    public bool IsAvailable => Remaining > 0;
    [ObservableProperty] private bool isSelected;
}
