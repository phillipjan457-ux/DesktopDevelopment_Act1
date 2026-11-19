using CommunityToolkit.Mvvm.ComponentModel;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class EquipmentCheckoutOption : ObservableObject
{
    public Equipment Equipment { get; }

    public string Label =>
        $"{Equipment.Name} (ID: {Equipment.EquipmentId})";

    [ObservableProperty]
    private bool isSelected;

    public EquipmentCheckoutOption(Equipment equipment)
    {
        Equipment = equipment;
    }
}