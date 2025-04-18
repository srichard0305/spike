﻿using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using spike.Database;
using spike.Models;
using spike.Services;

namespace spike.ViewModels;

public partial class FullAppointmentViewModel : PageViewModel
{
    [ObservableProperty]
    private Appointment _appointment;
    
    private readonly MainWindowViewModel _mainWindowViewModel;

    private DialogService _dialogService;

    public FullAppointmentViewModel()
    {
        
    }
    public FullAppointmentViewModel(AppointmentViewModel appointment, MainWindowViewModel mainWindowViewModel, DialogService dialogService)
    {
        Appointment = appointment.Appointment;
        _mainWindowViewModel = mainWindowViewModel;
        _dialogService = dialogService;
    }
    
    [RelayCommand]
    private async Task DeleteAppointment()
    {
        var confirmDialog = new ConfirmDialogViewModel()
        {
            Message = "Are you sure you want to delete this Appointment?",
        };
        await _dialogService.ShowDialog(_mainWindowViewModel, confirmDialog);
        if (confirmDialog.Confirmed)
        {
            UpdateDatabase.DeleteAppointment(Appointment);
            _mainWindowViewModel.CurrentPage = new HomePageViewModel(_mainWindowViewModel, _dialogService);
        }
        
    }

    [RelayCommand]
    private void NavigateToEditAppointment()
    {
        _mainWindowViewModel.CurrentPage = new EditAppointmentViewModel(Appointment, _mainWindowViewModel, _dialogService);
    }

    [RelayCommand]
    private void CheckIn()
    {
        Appointment.CheckIn = DateTime.Now.ToString("h:mm:ss tt");
        UpdateDatabase.UpdateCheckIn(Appointment);
    }
    
    [RelayCommand]
    private void CheckOut()
    {
        Appointment.CheckOut = DateTime.Now.ToString("h:mm:ss tt");
        UpdateDatabase.UpdateCheckOut(Appointment);
    }
    
    [RelayCommand]
    private void Cancelled()
    {
        Appointment.Cancelled = "True";
        UpdateDatabase.UpdateCancelled(Appointment);
    }
    
    [RelayCommand]
    private void NoShow()
    {
        Appointment.NoShow = "True";
        UpdateDatabase.UpdateNoShow(Appointment);
    }
}