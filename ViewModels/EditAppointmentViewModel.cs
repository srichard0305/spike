using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using spike.Data;
using spike.Database;
using spike.Models;
using spike.Services;

namespace spike.ViewModels;

public partial class EditAppointmentViewModel : PageViewModel
{
    
    [ObservableProperty]
    private Appointment _appointment;
    
    [ObservableProperty]
    private ObservableCollection<Employee> _employees;
    
    [ObservableProperty]
    private DateTimeOffset? _selectedDate;
    [ObservableProperty]
    private TimeSpan? _selectedStartTime;
    [ObservableProperty]
    private TimeSpan? _selectedEndTime;
    [ObservableProperty]
    private Employee? _selectedEmployeeStylist;
    [ObservableProperty]
    private Employee? _selectedEmployeeBookedBy;
    
    
    [ObservableProperty]
    private ObservableCollection<string> _errors;
    
    [ObservableProperty]
    private RequiredAppointmentFields _requiredFields;
    
    [ObservableProperty] 
    private string? _addedToDatabaseMessage;
    
    private readonly MainWindowViewModel _mainWindowViewModel;

    private DialogService _dialogService;

    public EditAppointmentViewModel(Appointment appointment, MainWindowViewModel mainWindowViewModel, DialogService dialogService)
    {
        
        Errors = new ObservableCollection<string>();
        InitErrors();
        InitEmployees();
        Appointment = appointment;
        SelectedDate = appointment.Date;
        SelectedStartTime = appointment.StartTime;
        SelectedEndTime = appointment.EndTime;
        
        SelectedEmployeeStylist = Employees.FirstOrDefault(e => 
            e.FirstName == appointment.EmployeeStylists.FirstName && e.LastName == appointment.EmployeeStylists.LastName); ;
        SelectedEmployeeBookedBy = Employees.FirstOrDefault(e => 
            e.FirstName == appointment.EmployeeBookedBy.FirstName && e.LastName == appointment.EmployeeBookedBy.LastName);
        
        _mainWindowViewModel = mainWindowViewModel;
        _dialogService = dialogService;
    }
    
    private void ValidateAppointmentInfo()
    {
        if (string.IsNullOrEmpty(Appointment.Service))
            Errors[(int)RequiredAppointmentFields.Service] = "Service is required";
        else if(Appointment.Service.Length > 150)
            Errors[(int)RequiredAppointmentFields.Service] = "Service is too long";
        
        if(string.IsNullOrEmpty(Appointment.Cost))
            Errors[(int)RequiredAppointmentFields.Cost] = "Cost is required";
        else if(!double.TryParse(Appointment.Cost, out _))
            Errors[(int)RequiredAppointmentFields.Cost] = "Cost is invalid";
        
        if(SelectedStartTime == null)
            Errors[(int)RequiredAppointmentFields.StartTime] = "Start Time is required";
        
        if(SelectedEndTime == null)
            Errors[(int)RequiredAppointmentFields.EndTime] = "End Time is required";

        if (SelectedDate == null)
            Errors[(int)RequiredAppointmentFields.Date] = "Date is required";
        
        if(Appointment.EmployeeStylists == null)
            Errors[(int)RequiredAppointmentFields.EmployeeStylists] = "Stylist is required";
        
        if(Appointment.EmployeeBookedBy == null)
            Errors[(int)RequiredAppointmentFields.EmployeeBookedBy] = "Booked by is required";

    }
    
    private void InitEmployees()
    {
        Employees = ReadFromDatabase.GetAllEmployees();
    }
    
    private void InitErrors()
    {
        foreach (RequiredAppointmentFields field in Enum.GetValues(typeof(RequiredAppointmentFields)))
        {
            Errors.Add(string.Empty);
        }
    }
    
    private void ResetErrors()
    {
        foreach (RequiredAppointmentFields field in Enum.GetValues(typeof(RequiredAppointmentFields)))
        {
            Errors[(int)field] = string.Empty;
        }
    }
    
    private bool DataValidation()
    {
        ResetErrors();
        ValidateAppointmentInfo();
        
        foreach (var message in Errors)
            if (message != string.Empty)
                return false;
        
        return true;
    }
    
    [RelayCommand]
    private async Task UpdateAppointment()
    {
        if (!DataValidation())
            return;
       
        ResetErrors();
        
        Appointment.StartTime = SelectedStartTime;
        Appointment.EndTime = SelectedEndTime;
        Appointment.Date = SelectedDate;
        Appointment.EmployeeStylists = SelectedEmployeeStylist;
        Appointment.EmployeeBookedBy = SelectedEmployeeBookedBy;

        AddedToDatabaseMessage = UpdateDatabase.UpdateAppointment(Appointment) ? "Appointment Updated!" : "Appointment cannot be updated!";
        await Task.Delay(1500);
        AddedToDatabaseMessage = "";

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
            if (UpdateDatabase.DeleteAppointment(Appointment))
            {
                AddedToDatabaseMessage = "Appointment Deleted!";
                await Task.Delay(1500);
                AddedToDatabaseMessage = "";
                _mainWindowViewModel.CurrentPage = new HomePageViewModel(_mainWindowViewModel, _dialogService);
            }
            else
            {
                AddedToDatabaseMessage = "Appointment cannot be deleted!";
                await Task.Delay(15000);
                AddedToDatabaseMessage = "";
            }
           
        }
    }
}