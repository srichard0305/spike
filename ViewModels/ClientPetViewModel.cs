using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.Input;
using spike.Database;
using spike.Models;
using spike.Data;

namespace spike.ViewModels;

public partial class ClientPetViewModel: PageViewModel
{
   
   public ClientPetViewModel()
   {
      PageTitle = AppPageNames.ClientPet;
   }
   
   /**********************  Add Client   ***********************************/
   // access text blocks and update view 
   private string? _firstName;
   [Required]
   public string? FirstName
   {
      get => _firstName;
      set => SetProperty(ref _firstName, value);
   }
    
   private string? _lastName;
   [Required]
   public string? LastName
   {
      get => _lastName;
      set => SetProperty(ref _lastName, value);
   }

   private string? _address;
    
   [Required]
   public string? Address
   {
      get => _address;
      set => SetProperty(ref _address, value);
   }
    
   private string? _etc;
   [Required]
   public string? Etc
   {
      get => _etc;
      set => SetProperty(ref _etc, value);
   }
    
   private string? _city;
    
   [Required]
   public string? City
   {
      get => _city;
      set => SetProperty(ref _city, value);
   }
    
   private string? _country;
    
   [Required]
   public string? Country
   {
      get => _country;
      set => SetProperty(ref _country, value);
   }

   private string? _province;
   [Required]
   public string? Province
   {
      get => _province;
      set => SetProperty(ref _province, value);
   }
    
   private string? _postalCode;
   [Required]
   public string? PostalCode
   {
      get => _postalCode;
      set => SetProperty(ref _postalCode, value);
   }
    
   private string? _phone;
    
   [Required]
   [Phone]
   public string? Phone
   {
      get => _phone;
      set => SetProperty(ref _phone, value);
   }
    
   private string? _email;
   [Required]
   [EmailAddress]
   public string? Email
   {
      get => _email;
      set => SetProperty(ref _email, value);
   }
   
   
   [RelayCommand]
   private void AddClient()
   {
      Client client = new();
      Address address = new();
      
      address.AddressLine = this.Address;
      address.Etc = this.Etc;
      address.City = this.City;
      address.PostalCode = this.PostalCode;
      address.Province = this.Province;
      address.Country = this.Country;
        
      client.FirstName = this.FirstName;
      client.LastName = this.LastName;
      client.Address = address;
      client.Email = this.Email;
      client.Phone = this.Phone;
      
      WriteToDatabase.AddClient(client);
   }
   
   /**********************************************************************************/
   
}
