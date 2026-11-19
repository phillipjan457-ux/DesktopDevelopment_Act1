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
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

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
            var connectionString =
    Environment.GetEnvironmentVariable("EQUIPMENT_BORROWING_CONNECTION")
    ?? throw new InvalidOperationException(
        "Set the EQUIPMENT_BORROWING_CONNECTION environment variable.");

            services.AddDbContext<EquipmentBorrowingDbContext>(options =>
            {
                options.UseNpgsql(connectionString);

#if DEBUG
                options.LogTo(
                    message => Debug.WriteLine(message),
                    new[] { DbLoggerCategory.Database.Command.Name },
                    LogLevel.Information);
#endif
            });

            services.AddScoped<IStudentRepository, EfStudentRepository>();
            services.AddScoped<IEquipmentRepository, EfEquipmentRepository>();
            services.AddScoped<IBorrowingRepository, EfBorrowingRepository>();
            services.AddScoped<IUnitOfWork, EfUnitOfWork>();


            services.AddScoped<BorrowEquipmentService>();
            services.AddScoped<ReturnEquipmentService>();
            services.AddScoped<RegisterStudentService>();
            services.AddScoped<RegisterEquipmentService>();
            services.AddScoped<UpdateEquipmentService>();
            services.AddScoped<UpdateStudentService>();
            services.AddScoped<CheckoutEquipmentService>();

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