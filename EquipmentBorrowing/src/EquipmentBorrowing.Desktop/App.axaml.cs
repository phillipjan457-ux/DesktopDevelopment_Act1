using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using EquipmentBorrowing.Desktop.ViewModels;
using EquipmentBorrowing.Desktop.Views;

namespace EquipmentBorrowing.Desktop;

public partial class App : Avalonia.Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var equipmentRepository = new EquipmentBorrowing.Infrastructure.Repositories.InMemoryEquipmentRepository();
            var studentRepository = new EquipmentBorrowing.Infrastructure.Repositories.InMemoryStudentRepository();
            var borrowingRepository = new EquipmentBorrowing.Infrastructure.Repositories.InMemoryBorrowingRepository();
            var borrowEquipmentService = new EquipmentBorrowing.Application.Services.BorrowEquipmentService(studentRepository, equipmentRepository, borrowingRepository);

            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(equipmentRepository, studentRepository, borrowEquipmentService),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}