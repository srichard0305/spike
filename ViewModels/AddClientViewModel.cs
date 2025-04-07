using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using spike.Data;
using spike.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using spike.Database;
using spike.Services;

namespace spike.ViewModels;

public partial class AddClientViewModel : PageViewModel
{ 
    [ObservableProperty]
    private Client _client;
    [ObservableProperty]
    private Address _address;
    [ObservableProperty]
    private ContactInfo _contactInfo;
    [ObservableProperty]
    private ObservableCollection<Pet> _pets;
    [ObservableProperty]
    private DateTimeOffset? _selectedBirthday;
    
    [ObservableProperty]
    private ObservableCollection<string> _provinces;
    
    [ObservableProperty]
    private ObservableCollection<string> _genders;
    
    [ObservableProperty]
    private ObservableCollection<string> _yesNo;
    
    [ObservableProperty]
    private ObservableCollection<string> _errors;
    
    [ObservableProperty]
    private ObservableCollection<string> _ages;
    
    [ObservableProperty]
    private RequiredFieldsEnum _requiredFields;

    [ObservableProperty] 
    private string? _addedToDatabaseMessage;
    
    private readonly MainWindowViewModel _mainWindowViewModel;

    private DialogService _dialogService;
    
    public AddClientViewModel(MainWindowViewModel mainWindowViewModel, DialogService dialogService) 
    { 
        PageTitle = AppPageNames.AddClient;
        Client = new Client();
        Address = new Address();
        ContactInfo = new ContactInfo();
        Pets = new ObservableCollection<Pet>();
        Errors = new ObservableCollection<string>();
        Provinces = new ObservableCollection<string>()
        {
            "AB", "BC", "MB", "NB", "NL", "NT",
            "NS", "NU", "ON", "PE", "QC", "SK",
            "YT"
        };
        Ages = new ObservableCollection<string>()
        {
            "1-6 Months", "6-12 Months", "1", "2", "3", "4", "5", "6", "7", "8", "9",
            "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20",
            "21", "22", "23", "24", "25"
        };
        Genders = new ObservableCollection<string>()
        {
            "Male",
            "Female"
        };
        YesNo = new ObservableCollection<string>()
        {
            "Yes",
            "No"
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

    private void ResetErrors()
    {
        foreach (RequiredFieldsEnum field in Enum.GetValues(typeof(RequiredFieldsEnum)))
        {
            Errors[(int)field] = string.Empty;
        }
    }
    
    private bool DataValidation()
    {
        ResetErrors();
        ValidateClientInfo();
        ValidateContactInfo(); 
        ValidatePetInfo();
        
        foreach (var message in Errors)
            if (message != string.Empty)
            {
                Debug.WriteLine(message);
                return false;
            }
                
        
        return true;
    }

    private void ValidateClientInfo()
    {
        if (string.IsNullOrEmpty(Client.FirstName))
            Errors[(int)RequiredFieldsEnum.ClientFirstName] = "First name is required";
        else if(Client.FirstName.Length > 50)
            Errors[(int)RequiredFieldsEnum.ClientFirstName] = "First name is too long";
        
        if (string.IsNullOrEmpty(Client.LastName))
            Errors[(int)RequiredFieldsEnum.ClientLastName] = "Last name is required";
        else if(Client.LastName.Length > 50)
            Errors[(int)RequiredFieldsEnum.ClientLastName] = "Last name is too long";
        
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

        if (!string.IsNullOrEmpty(ContactInfo.Email))
        {
            if (!Regex.Match(ContactInfo.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase).Success)
                Errors[(int)RequiredFieldsEnum.ClientValidEmail] = "Email is invalid";
        }
    }

    private void ValidatePetInfo()
    {
        foreach (Pet pet in Pets)
        {
            if (string.IsNullOrEmpty(pet.Name))
                Errors[(int)RequiredFieldsEnum.PetName] = "Name is required";
            else if(pet.Name.Length > 150)
                Errors[(int)RequiredFieldsEnum.PetName] = "Name is too long";
            
            if (string.IsNullOrEmpty(pet.Breed))
                Errors[(int)RequiredFieldsEnum.PetBreed] = "Breed is required";
            else if(pet.Breed.Length > 150)
                Errors[(int)RequiredFieldsEnum.PetBreed] = "Breed is too long";
            
            if (string.IsNullOrEmpty(pet.Age))
                Errors[(int)RequiredFieldsEnum.PetAge] = "Age is required";
            
            if (string.IsNullOrEmpty(pet.Gender))
                Errors[(int)RequiredFieldsEnum.PetGender] = "Gender is required";
            
            if (string.IsNullOrEmpty(pet.SpayedNeutered))
                Errors[(int)RequiredFieldsEnum.PetSpayedOrNeutered] = "Spayed or neutered is required";
            
            if (!string.IsNullOrEmpty(pet.VetPhone))
            {
                if (!Regex.Match(pet.VetPhone, @"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}").Success)
                    Errors[(int)RequiredFieldsEnum.PetVetPhone] = "Phone number is invalid";
            }

            if (SelectedBirthday != null)
            {
                pet.Birthday = SelectedBirthday.ToString();
            }
            
        }
    }
    
    
   [RelayCommand]
   private async Task AddClient()
   {
       if (!DataValidation())
           return;

       Client.Address = Address;
       Client.ContactInfo = ContactInfo;
       Client.Pets = Pets;
       
       ResetErrors();

       if (WriteToDatabase.AddClient(Client))
       {
           AddedToDatabaseMessage = "Client added!";
           await Task.Delay(1500);
           AddedToDatabaseMessage = "";
           Client = new Client();
           Address = new Address();
           ContactInfo = new ContactInfo();
           Pets = new ObservableCollection<Pet>();
       }
       else
       {
           AddedToDatabaseMessage = "Client cannot be added!";
           await Task.Delay(1500);
           AddedToDatabaseMessage = "";
       }
       
   }

   [RelayCommand]
   private void AddPet()
   {
       Pets.Add(new Pet());
   }
   
   [RelayCommand]
   private async Task DeletePet(Pet pet)
   {
       var confirmDialog = new ConfirmDialogViewModel()
       {
           Message = "Are you sure you want to delete this pet?",
       };
       await _dialogService.ShowDialog(_mainWindowViewModel, confirmDialog);
       if(confirmDialog.Confirmed) 
           Pets.Remove(pet);
   }
   

}