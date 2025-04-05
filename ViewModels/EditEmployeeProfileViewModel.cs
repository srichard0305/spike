using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using spike.Data;
using spike.Database;
using spike.Models;
using spike.Services;

namespace spike.ViewModels;

public partial class EditEmployeeProfileViewModel : PageViewModel
{
    [ObservableProperty]
    private Employee _employee;
    
    [ObservableProperty]
    private ObservableCollection<string> _provinces;
    
    [ObservableProperty]
    private ObservableCollection<string> _errors;
    
    [ObservableProperty]
    private RequiredFieldsEnum _requiredFields;

    [ObservableProperty] 
    private string? _addedToDatabaseMessage;
    
    private readonly MainWindowViewModel _mainWindowViewModel;
    
    private DialogService _dialogService;
    
    public EditEmployeeProfileViewModel(Employee employee, MainWindowViewModel mainWindowViewModel, DialogService dialogService)
    {
        PageTitle = AppPageNames.EditEmployeeProfile;
        Employee = employee;
        
        Errors = new ObservableCollection<string>();
        
        Provinces = new ObservableCollection<string>()
        {
            "AB", "BC", "MB", "NB", "NL", "NT",
            "NS", "NU", "ON", "PE", "QC", "SK",
            "YT"
        };

        InitErrors();
        
        _addedToDatabaseMessage = "";
        _mainWindowViewModel = mainWindowViewModel;
        _dialogService = dialogService;
    }
    
    private void InitErrors()
    {
        foreach (RequiredFieldsEnum field in Enum.GetValues(typeof(RequiredFieldsEnum)))
        {
            Errors.Add(string.Empty);
        }
    }
    
     private bool DataValidation()
    {
        InitErrors();
        ValidateEmployeeInfo();
        ValidateAddressInfo();
        ValidateContactInfo(); 
        
        foreach (var message in Errors)
            if (message != string.Empty)
                return false;
        
        return true;
    }

    private void ValidateEmployeeInfo()
    {
        if (string.IsNullOrEmpty(Employee.FirstName))
            Errors[(int)RequiredFieldsEnum.ClientFirstName] = "First name is required";
        else if(Employee.FirstName.Length > 50)
            Errors[(int)RequiredFieldsEnum.ClientFirstName] = "First name is too long";
        
        if (string.IsNullOrEmpty(Employee.LastName))
            Errors[(int)RequiredFieldsEnum.ClientLastName] = "Last name is required";
        else if(Employee.LastName.Length > 50)
            Errors[(int)RequiredFieldsEnum.ClientLastName] = "Last name is too long";
        
        if (!string.IsNullOrEmpty(Employee.Cardinality))
        {
            if(Employee.Cardinality.Length > 50)
                Errors[(int)RequiredFieldsEnum.Cardinality] = "Invalid";
        }
        
        if (!string.IsNullOrEmpty(Employee.Commission))
        {
            if(Employee.Commission.Length > 50)
                Errors[(int)RequiredFieldsEnum.Commission] = "Invalid";
        }
        
        if (!string.IsNullOrEmpty(Employee.BasePay))
        {
            if(Employee.BasePay.Length > 50)
                Errors[(int)RequiredFieldsEnum.BasePay] = "Invalid";
        }
        
    }
    
    private void ValidateAddressInfo()
    {
        if (string.IsNullOrEmpty(Employee.Address.AddressLine))
            Errors[(int)RequiredFieldsEnum.ClientAddress] = "Address is required";
        else if(Employee.Address.AddressLine.Length > 150)
            Errors[(int)RequiredFieldsEnum.ClientLastName] = "Address is too long";
        
        if (string.IsNullOrEmpty(Employee.Address.City))
            Errors[(int)RequiredFieldsEnum.ClientCity] = "City is required";
        else if(Employee.Address.City.Length > 150)
            Errors[(int)RequiredFieldsEnum.ClientCity] = "City is too long";
        
        if (string.IsNullOrEmpty(Employee.Address.Province))
            Errors[(int)RequiredFieldsEnum.ClientProvince] = "Province is required";
        
        if (string.IsNullOrEmpty(Employee.Address.Country))
            Errors[(int)RequiredFieldsEnum.ClientCountry] = "Country is required";
        else if(Employee.Address.Country.Length > 150)
            Errors[(int)RequiredFieldsEnum.ClientCountry] = "Country is too long";
        
        if (string.IsNullOrEmpty(Employee.Address.PostalCode))
            Errors[(int)RequiredFieldsEnum.ClientPostalCode] = "Postal Code is required";
        else if(Employee.Address.PostalCode.Length > 10)
            Errors[(int)RequiredFieldsEnum.ClientPostalCode] = "Postal Code is too long";
    }

    private void ValidateContactInfo()
    {
        if (string.IsNullOrEmpty(Employee.ContactInfo.PrimaryPhone))
            Errors[(int)RequiredFieldsEnum.ClientPhoneNumber] = "Phone is required";
        // if not in a valid phone number format
        else if (!Regex.Match(Employee.ContactInfo.PrimaryPhone, @"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}").Success)
            Errors[(int)RequiredFieldsEnum.ClientPhoneNumber] = "Phone number is invalid";
        
        if (!string.IsNullOrEmpty(Employee.ContactInfo.SecondaryPhone))
        {
            if (!Regex.Match(Employee.ContactInfo.SecondaryPhone, @"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}").Success)
                Errors[(int)RequiredFieldsEnum.ClientSecondaryPhone] = "Phone number is invalid";
        }
        
        if (!string.IsNullOrEmpty(Employee.ContactInfo.EmergencyPhone))
        {
            if (!Regex.Match(Employee.ContactInfo.EmergencyPhone, @"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}").Success)
                Errors[(int)RequiredFieldsEnum.ClientEmergPhone] = "Phone number is invalid";
        }
        
        if (string.IsNullOrEmpty(Employee.ContactInfo.Email))
        {
            Errors[(int)RequiredFieldsEnum.ClientValidEmail] = "Email is required";
        }
        else if(!Regex.Match(Employee.ContactInfo.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase).Success)
            Errors[(int)RequiredFieldsEnum.ClientValidEmail] = "Email is invalid";
    }
    
   [RelayCommand]
   private async Task UpdateEmployee()
   {
       if (!DataValidation())
           return;
       
       InitErrors();
       
       AddedToDatabaseMessage = UpdateDatabase.UpdateEmployee(Employee) ? "Employee Updated!" : "Employee cannot be updated!";
       await Task.Delay(1500);
       AddedToDatabaseMessage = "";

   }
   

   [RelayCommand]
   private async Task DeleteEmployee()
   {
       var confirmDialog = new ConfirmDialogViewModel()
       {
           Message = "Are you sure you want to delete this Employee?",
       };
       await _dialogService.ShowDialog(_mainWindowViewModel, confirmDialog);
       if (confirmDialog.Confirmed)
       {
           if (UpdateDatabase.DeleteEmployee(Employee))
           {
               AddedToDatabaseMessage = "Employee Deleted!";
               await Task.Delay(1500);
               AddedToDatabaseMessage = "";
               _mainWindowViewModel.CurrentPage = new EmployeePageViewModel(_mainWindowViewModel, _dialogService);
           }
           else
           {
               AddedToDatabaseMessage = "Employee cannot be deleted!";
               await Task.Delay(15000);
               AddedToDatabaseMessage = "";
           }
           
       }
       
   }
}