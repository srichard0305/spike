using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.Input;
using spike.Data;
using spike.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using spike.Database;


namespace spike.ViewModels;

public partial class AddClientViewModel : PageViewModel
{ 
    public AddClientViewModel() 
    { 
        PageTitle = AppPageNames.AddClient; 
    }

    [ObservableProperty]
    private string? _firstName;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FirstNameIsRequired))]
    private string? _firstNameErrorMessage = "First Name is required";
    [ObservableProperty]
    private bool _firstNameIsRequired;

    [ObservableProperty]
    private string? _lastName;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(LastNameIsRequired))]
    private string? _lastNameErrorMessage = "Last Name is required";
    [ObservableProperty]
    private bool _lastNameIsRequired;
    
    [ObservableProperty]
    private string? _address;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AddressIsRequired))]
    private string? _addressErrorMessage = "Address is required";
    [ObservableProperty]
    private bool _addressIsRequired;
 
   [ObservableProperty]
   private string? _etc;
 
   [ObservableProperty]
   private string? _city;
   [ObservableProperty]
   [NotifyPropertyChangedFor(nameof(CityIsRequired))]
   private string? _cityErrorMessage = "City is required";
   [ObservableProperty]
   private bool _cityIsRequired;

   [ObservableProperty]
   private string? _country;
   [ObservableProperty]
   [NotifyPropertyChangedFor(nameof(CountryIsRequired))]
   private string? _countryErrorMessage = "Country is required";
   [ObservableProperty]
   private bool _countryIsRequired;
  
   [ObservableProperty]
   private string? _province;
   [ObservableProperty]
   [NotifyPropertyChangedFor(nameof(ProvinceIsRequired))]
   private string? _provinceErrorMessage = "Province is required";
   [ObservableProperty]
   private bool _provinceIsRequired;
   
   [ObservableProperty]
   private string? _postalCode;
   [ObservableProperty]
   [NotifyPropertyChangedFor(nameof(PostalCodeIsRequired))]
   private string? _postalCodeErrorMessage = "Postal Code is required";
   [ObservableProperty]
   private bool _postalCodeIsRequired;

   [ObservableProperty]
   private string? _phone;
   [ObservableProperty]
   [NotifyPropertyChangedFor(nameof(PhoneIsRequired))]
   private string? _phoneErrorMessage = "Phone Number is required";
   [ObservableProperty]
   private bool _phoneIsRequired;
  
   [ObservableProperty]
   private string? _email;
   [ObservableProperty]
   [NotifyPropertyChangedFor(nameof(ValidEmailIsRequired))]
   private string? _emailErrorMessage = "Valid email is required";
   [ObservableProperty]
   private bool _validEmailIsRequired;
   
   
   [RelayCommand]
   private void AddClient()
   {
      Client client = new();
      Address address = new();
      
     // data validation 
      if (string.IsNullOrEmpty(Address))
      {
          AddressIsRequired = true;
          return;
      }
      address.AddressLine = Address;
      AddressIsRequired = false;
      
      address.Etc = Etc;
      
      if (string.IsNullOrEmpty(City))
      {
          CityIsRequired = true;
          return;
      }
      address.City = City;
      CityIsRequired = false;
      
      if (string.IsNullOrEmpty(PostalCode))
      {
          PostalCodeIsRequired = true;
          return;
      }
      address.PostalCode = PostalCode;
      PostalCodeIsRequired = false;
      
      if (string.IsNullOrEmpty(Province))
      {
          ProvinceIsRequired = true;
          return;
      }
      address.Province = Province;
      ProvinceIsRequired = false;
      
      if (string.IsNullOrEmpty(Country))
      {
          CountryIsRequired = true;
          return;
      }
      address.Country = Country;
      CountryIsRequired = false;

      if (string.IsNullOrEmpty(FirstName))
      {
          FirstNameIsRequired = true;
          return;
      }
      client.FirstName = FirstName;
      FirstNameIsRequired = false;
  
      if (string.IsNullOrEmpty(LastName))
      {
          LastNameIsRequired = true;
          return;
      }
      client.LastName = LastName;
      LastNameIsRequired = false;
      
      client.Address = address;
      
      if (!string.IsNullOrEmpty(Email))
      {
          if (!Regex.Match(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase).Success)
          {
              ValidEmailIsRequired= true;
              return;
          }
         
          client.Email = Email;
          ValidEmailIsRequired= false;
      }
      
      if (string.IsNullOrEmpty(Phone))
      {
          LastNameIsRequired = true;
          return;
      }
      if (!Regex.Match(Phone, @"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}").Success)
      {
          PhoneIsRequired = true;
          PhoneErrorMessage = "Invalid phone number";
          return;
      }
      client.Phone = Phone;
      PhoneErrorMessage = "Phone Number is required";
      PhoneIsRequired = true;

      if (WriteToDatabase.AddClient(client))
      {
          //todo write message to user that it was successfully added to database
      }
      else
      {
          // todo wrtie message saying it did not work
      }
      
   }
   

}