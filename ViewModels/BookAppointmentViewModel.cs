using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using spike.Data;
using spike.Database;
using spike.Models;
using spike.Services;


namespace spike.ViewModels;
public partial class BookAppointmentViewModel: PageViewModel
{
    [ObservableProperty]
    private ObservableCollection<Client> _clients;
    
    [ObservableProperty]
    private ObservableCollection<Employee> _employees;

    [ObservableProperty]
    private Client _selectedClient;
    
    [ObservableProperty]
    private Pet _selectedPet;
    
    [ObservableProperty]
    private DateTimeOffset? _selectedDate;
    [ObservableProperty]
    private TimeSpan? _selectedStartTime;
    [ObservableProperty]
    private TimeSpan? _selectedEndTime;
    
    
    [ObservableProperty]
    private Appointment _appointment;

    [ObservableProperty] 
    private bool _changePetButton;
    
    [ObservableProperty]
    private ObservableCollection<string> _errors;
    
    [ObservableProperty]
    private RequiredAppointmentFields _requiredFields;

    [ObservableProperty] 
    private string? _addedToDatabaseMessage;
    
    private MainWindowViewModel _mainWindowViewModel;
    
    private DialogService _dialogService;

    public BookAppointmentViewModel(MainWindowViewModel mainWindowViewModel, DialogService dialogService)
    {
        PageTitle = AppPageNames.BookAppointment;
        Clients = new ObservableCollection<Client>();
        Errors = new ObservableCollection<string>();
        Appointment = new Appointment();
        InitClients();
        InitEmployees();
        InitErrors();
        _mainWindowViewModel = mainWindowViewModel;
        _dialogService = dialogService;
    }

    private void InitClients()
    {
        Clients = ReadFromDatabase.GetAllClients();
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

    partial void OnSelectedClientChanged(Client value)
    {
        if (value != null)
        {
            GetSelectedPet(value.Pets);
        }    
    }

    // open dialog and select pet
    private async Task GetSelectedPet(ObservableCollection<Pet> pets)
    {
        var selectedPetDialog = new SelectedPetDialogViewModel(pets)
        {
            Message = "Select a pet",
        };
        await _dialogService.ShowDialog(_mainWindowViewModel, selectedPetDialog);
        if (selectedPetDialog.Confirmed)
        {
           SelectedPet = selectedPetDialog.SelectedPet;
           ChangePetButton = true;
        }
       
    }

    [RelayCommand]
    private async Task ChangeSelectedPet()
    {
        await GetSelectedPet(SelectedClient.Pets);
    }

    [RelayCommand]
    private async Task BookAppointment()
    {
        if (SelectedClient == null)
        {
            AddedToDatabaseMessage = "Please select a client";
            return;
        }
        
        if(!DataValidation())
            return;
        
        Appointment.Client = SelectedClient;
        Appointment.Pet = SelectedPet;
        Appointment.Date = SelectedDate;
        Appointment.StartTime = SelectedStartTime;
        Appointment.EndTime = SelectedEndTime;
        
        ResetErrors();

        if (WriteToDatabase.AddAppointment(Appointment))
        {
            AddedToDatabaseMessage = "Appointment Booked!";
            await Task.Delay(1500);
            AddedToDatabaseMessage = "";
        }
        else
        {
            AddedToDatabaseMessage = "Appointment could not be booked!";
            await Task.Delay(1500);
            AddedToDatabaseMessage = "";
        }

    }
}