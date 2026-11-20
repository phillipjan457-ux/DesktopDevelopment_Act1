using CareFlow.Application;
using CareFlow.Domain;
using CareFlow.Infrastructure;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace CareFlow.Desktop.ViewModels;

public partial class MainWindowViewModel(BookingService bookingService, CareFlowDbContext db) : ObservableObject
{
    public ObservableCollection<Patient> Patients { get; } = new();
    public ObservableCollection<ServiceChoice> AvailableServices { get; } = new();
    public ObservableCollection<BookingSummary> Bookings { get; } = new();
    [ObservableProperty] private Patient? selectedPatient;
    [ObservableProperty] private DateTimeOffset? selectedDate = DateTimeOffset.Now.Date;
    [ObservableProperty] private string statusMessage = "Preparing CareFlow database…";
    private bool _initialized;

    public async Task InitializeAsync()
    {
        if (_initialized) return;
        _initialized = true;
        try
        {
            await db.Database.MigrateAsync();
            await CareFlowSeed.InitializeAsync(db);
            Patients.Clear();
            foreach (var patient in await bookingService.GetPatientsAsync()) Patients.Add(patient);
            await RefreshAsync();
            StatusMessage = "Ready. Choose a patient, date, and one to three services.";
        }
        catch (Exception ex) { StatusMessage = $"Could not initialize the database: {ex.GetBaseException().Message}"; }
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        try
        {
            var date = DateOnly.FromDateTime((SelectedDate ?? DateTimeOffset.Now).DateTime);
            AvailableServices.Clear();
            foreach (var item in await bookingService.GetAvailabilityAsync(date)) AvailableServices.Add(new ServiceChoice(item));
            Bookings.Clear();
            foreach (var item in await bookingService.GetBookingsAsync()) Bookings.Add(item);
        }
        catch (Exception ex) { StatusMessage = $"Could not load booking information: {ex.GetBaseException().Message}"; }
    }

    [RelayCommand]
    private async Task ConfirmAsync()
    {
        var selected = AvailableServices.Where(s => s.IsSelected).ToList();
        try
        {
            var id = await bookingService.ConfirmAsync(SelectedPatient?.PatientId,
                DateOnly.FromDateTime((SelectedDate ?? DateTimeOffset.Now).DateTime), selected.Select(s => s.Id).ToList());
            StatusMessage = $"Booking #{id} confirmed for {SelectedPatient!.FullName}.";
            await RefreshAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = ex is InvalidOperationException ? ex.Message : $"Booking failed and nothing was saved: {ex.GetBaseException().Message}";
            await RefreshAsync();
        }
    }

    partial void OnSelectedDateChanged(DateTimeOffset? value)
    {
        if (_initialized) _ = RefreshAsync();
    }
}
