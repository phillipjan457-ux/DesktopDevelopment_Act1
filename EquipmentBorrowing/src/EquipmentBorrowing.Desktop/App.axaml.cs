using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Desktop.Services;
using EquipmentBorrowing.Desktop.ViewModels;
using EquipmentBorrowing.Desktop.Views;
using EquipmentBorrowing.Infrastructure.Persistence;
using EquipmentBorrowing.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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
            var services = new ServiceCollection();

            // One context shared by the services within each operation.
            services.AddDbContext<EquipmentBorrowingDbContext>(options =>
                options.UseSqlite(EquipmentDatabase.GetConnectionString()));

            services.AddScoped<IStudentRepository, EfStudentRepository>();
            services.AddScoped<IEquipmentRepository, EfEquipmentRepository>();
            services.AddScoped<IBorrowingRepository, EfBorrowingRepository>();
            services.AddScoped<IUnitOfWork, EfUnitOfWork>();

            services.AddScoped<BorrowEquipmentService>();
            services.AddScoped<ReturnEquipmentService>();

            // Creates and disposes a scope for each operation.
            services.AddSingleton<
                IEquipmentBorrowingOperations,
                ScopedEquipmentBorrowingOperations>();

            services.AddTransient<MainWindowViewModel>();

            var provider = services.BuildServiceProvider(
                new ServiceProviderOptions
                {
                    ValidateScopes = true,
                    ValidateOnBuild = true
                });

            desktop.Exit += (_, _) => provider.Dispose();

            desktop.MainWindow = new MainWindow
            {
                DataContext = provider.GetRequiredService<MainWindowViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}