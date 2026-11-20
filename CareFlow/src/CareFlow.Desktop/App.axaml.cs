using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CareFlow.Application;
using CareFlow.Desktop.ViewModels;
using CareFlow.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace CareFlow.Desktop;

public partial class App : Avalonia.Application
{
    private ServiceProvider? _provider;
    private IServiceScope? _scope;

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CareFlow");
            Directory.CreateDirectory(folder);
            var databasePath = Path.Combine(folder, "careflow.db");
            var services = new ServiceCollection();
            services.AddDbContext<CareFlowDbContext>(options => options.UseSqlite($"Data Source={databasePath}"));
            services.AddScoped<ICareFlowRepository, CareFlowRepository>();
            services.AddScoped<BookingService>();
            services.AddScoped<MainWindowViewModel>();
            _provider = services.BuildServiceProvider();
            _scope = _provider.CreateScope();
            var viewModel = _scope.ServiceProvider.GetRequiredService<MainWindowViewModel>();
            var window = new Views.MainWindow { DataContext = viewModel };
            window.Opened += async (_, _) => await viewModel.InitializeAsync();
            desktop.MainWindow = window;
            desktop.Exit += (_, _) => { _scope?.Dispose(); _provider?.Dispose(); };
        }
        base.OnFrameworkInitializationCompleted();
    }
}
