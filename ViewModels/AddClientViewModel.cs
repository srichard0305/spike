using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.Input;
using spike.Data;
using spike.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using spike.Database;


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
    private ObservableCollection<string> _errors;
    [ObservableProperty]
    private ObservableCollection<Pet> _pets;
    [ObservableProperty]
    private ObservableCollection<Client> _clients;
    
    RequiredFieldsEnum _requiredFields;
    
    public AddClientViewModel() 
    { 
        Client = new Client();
        Address = new Address();
        ContactInfo = new ContactInfo();
        Errors = new ObservableCollection<string>();
        Pets = new ObservableCollection<Pet>();
        Clients = new ObservableCollection<Client>();
        InitErrors();
        InitClients();
        PageTitle = AppPageNames.AddClient; 
    }

    private void InitErrors()
    {
        foreach (RequiredFieldsEnum field in Enum.GetValues(typeof(RequiredFieldsEnum)))
        {
            Errors.Add(string.Empty);
        }
    }

    private void InitClients()
    {
        // TODO grab all clients from database for autocomplete box
    }
    
   [RelayCommand]
   private void AddClient()
   {
       Errors[0] = "Errors";
       /*
      if (!string.IsNullOrEmpty(Email))
      {
          if (!Regex.Match(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase).Success)
          {
              ValidEmailIsRequired= true;

          }
          else
          {
              client.Email = Email;
              ValidEmailIsRequired= false;
          }
      }

      if (string.IsNullOrEmpty(Phone))
      {
          PhoneIsRequired = true;
      }
      else
      {
          if (!Regex.Match(Phone, @"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}").Success)
          {
              PhoneIsRequired = true;
              PhoneErrorMessage = "Invalid phone number";
          }
          else
          {
              client.Phone = Phone;
              PhoneIsRequired = true;
          }
      }

      // if any field is empty or null return the function and display error messages
      if (AddressIsRequired ||
          CityIsRequired ||
          PostalCodeIsRequired ||
          ProvinceIsRequired ||
          CountryIsRequired ||
          FirstNameIsRequired ||
          LastNameIsRequired ||
          PhoneIsRequired ||
          ValidEmailIsRequired)
          return;


      //write to database
      if (WriteToDatabase.AddClient(client))
      {
          //todo write message to user that it was successfully added to database
          AddedToDatabase = true;
          AddedToDatabaseMessage = "Client added!";
      }
      else
      {
          // todo wrtie message saying it did not work
          AddedToDatabase = true;
          AddedToDatabaseMessage = "Client cannot be added!";
      }

      AddressIsRequired = false;
      CityIsRequired = false;
      PostalCodeIsRequired = false;
      ProvinceIsRequired = false;
      CountryIsRequired = false;
      FirstNameIsRequired = false;
      LastNameIsRequired = false;
      PhoneIsRequired = false;
      ValidEmailIsRequired= false;
      */
   }

   [RelayCommand]
   private void AddPet()
   {
       Pets.Add(new Pet());
   }
   
   [RelayCommand]
   private void DeletePet(Pet pet)
   {
       Pets.Remove(pet);
   }
   

}