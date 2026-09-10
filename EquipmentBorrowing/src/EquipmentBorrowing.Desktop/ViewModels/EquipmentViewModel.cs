using CommunityToolkit.Mvvm.ComponentModel;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class EquipmentViewModel : ViewModelBase
{
    private readonly IEquipmentRepository _equipmentRepository;

    [ObservableProperty]
    private ObservableCollection<Equipment> equipmentList = new();

    public EquipmentViewModel(IEquipmentRepository equipmentRepository)
    {
        _equipmentRepository = equipmentRepository;
        _ = LoadEquipmentAsync();
    }

    private async Task LoadEquipmentAsync()
    {
        var items = await _equipmentRepository.GetAllEquipmentAsync();
        EquipmentList = new ObservableCollection<Equipment>(items);
    }
}