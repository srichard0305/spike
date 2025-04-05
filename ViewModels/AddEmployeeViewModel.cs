using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using spike.Data;
using spike.Database;
using spike.Models;
using spike.Services;

namespace spike.ViewModels;

public partial class AddEmployeeViewModel : PageViewModel
{
    [ObservableProperty]
    private Employee _employee;
    [ObservableProperty]
    private Address _address;
    [ObservableProperty]
    private ContactInfo _contactInfo;
    
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
    
    public AddEmployeeViewModel(MainWindowViewModel mainWindowViewModel, DialogService dialogService) 
    { 
        PageTitle = AppPageNames.AddClient;
        Employee = new Employee();
        Address = new Address();
        ContactInfo = new ContactInfo();
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
        if (string.IsNullOrEmpty(Address.AddressLine))
            Errors[(int)RequiredFieldsEnum.ClientAddress] = "Address is required";
        else if(Address.AddressLine.Length > 150)
            Errors[(int)RequiredFieldsEnum.ClientLastName] = "Address is too long";
        
        if (string.IsNullOrEmpty(Address.City))
            Errors[(int)RequiredFieldsEnum.ClientCity] = "City is required";
        else if(Address.City.Length > 150)
            Errors[(int)RequiredFieldsEnum.ClientCity] = "City is too long";
        
        if (string.IsNullOrEmpty(Address.Province))
            Errors[(int)RequiredFieldsEnum.ClientProvince] = "Province is required";
        
        if (string.IsNullOrEmpty(Address.Country))
            Errors[(int)RequiredFieldsEnum.ClientCountry] = "Country is required";
        else if(Address.Country.Length > 150)
            Errors[(int)RequiredFieldsEnum.ClientCountry] = "Country is too long";
        
        if (string.IsNullOrEmpty(Address.PostalCode))
            Errors[(int)RequiredFieldsEnum.ClientPostalCode] = "Postal Code is required";
        else if(Address.PostalCode.Length > 10)
            Errors[(int)RequiredFieldsEnum.ClientPostalCode] = "Postal Code is too long";
    }

    private void ValidateContactInfo()
    {
        if (string.IsNullOrEmpty(ContactInfo.PrimaryPhone))
            Errors[(int)RequiredFieldsEnum.ClientPhoneNumber] = "Phone is required";
        // if not in a valid phone number format
        else if (!Regex.Match(ContactInfo.PrimaryPhone, @"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}").Success)
            Errors[(int)RequiredFieldsEnum.ClientPhoneNumber] = "Phone number is invalid";
        
        if (!string.IsNullOrEmpty(ContactInfo.SecondaryPhone))
        {
            if (!Regex.Match(ContactInfo.SecondaryPhone, @"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}").Success)
                Errors[(int)RequiredFieldsEnum.ClientSecondaryPhone] = "Phone number is invalid";
        }
        
        if (!string.IsNullOrEmpty(ContactInfo.EmergencyPhone))
        {
            if (!Regex.Match(ContactInfo.EmergencyPhone, @"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}").Success)
                Errors[(int)RequiredFieldsEnum.ClientEmergPhone] = "Phone number is invalid";
        }

        if (string.IsNullOrEmpty(ContactInfo.Email))
        {
            Errors[(int)RequiredFieldsEnum.ClientValidEmail] = "Email is required";
        }
        else if(!Regex.Match(ContactInfo.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase).Success)
            Errors[(int)RequiredFieldsEnum.ClientValidEmail] = "Email is invalid";
    }
    
   [RelayCommand]
   private async Task AddEmployee()
   {
       if (!DataValidation())
           return;

       Employee.Address = Address;
       Employee.ContactInfo = ContactInfo;
       
       InitErrors();

       AddedToDatabaseMessage = WriteToDatabase.AddEmployee(Employee) ? "Employee added!" : "Employee cannot be added!";
       await Task.Delay(1500);
       AddedToDatabaseMessage = "";

   }
   
}