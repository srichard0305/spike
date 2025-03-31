using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.Sqlite;
using spike.Database;
using spike.Models;
using spike.Data;
using spike.Factories;
using spike.Views;

namespace spike.ViewModels;

public partial class ClientPetViewModel: PageViewModel
{
   
   [ObservableProperty]
   private ObservableCollection<Client>? _clients;

   [ObservableProperty] 
   private bool _showAddButton;

   private readonly MainWindowViewModel _mainWindowViewModel;
   
   //default constructor for designer remove after dev 
   public ClientPetViewModel()
   {
      PageTitle = AppPageNames.ClientPet;
      Clients = new ObservableCollection<Client>();
      ShowAddButton = true;
      InitClientsList();
   }
   
   public ClientPetViewModel(MainWindowViewModel mainWindowViewModel) {
      _mainWindowViewModel = mainWindowViewModel;
      PageTitle = AppPageNames.ClientPet;
      Clients = new ObservableCollection<Client>();
      ShowAddButton = true;
      InitClientsList();
   }
   
   private void InitClientsList()
   {
      Clients = ReadFromDatabase.GetAllClients();
   }
   

   [RelayCommand]
   private void NavigateToAddClient()
   {
      ShowAddButton = false;
      _mainWindowViewModel.CurrentPage = new AddClientViewModel();
      
   }

   [RelayCommand]
   private void NavigateToClientProfile(Client client)
   {
      ShowAddButton = false;
      _mainWindowViewModel.CurrentPage = new ClientProfileViewModel(client, _mainWindowViewModel);
   }
   
      

}
