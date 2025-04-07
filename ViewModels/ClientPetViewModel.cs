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
using spike.Services;
using spike.Views;

namespace spike.ViewModels;

public partial class ClientPetViewModel: PageViewModel
{
   
   [ObservableProperty]
   private ObservableCollection<Client>? _clients;
   
   [ObservableProperty] 
   private bool _showAddButton;

   private readonly MainWindowViewModel _mainWindowViewModel;
   
   private DialogService _dialogService;
   
   public ClientPetViewModel(MainWindowViewModel mainWindowViewModel, DialogService dialogService) {
      _mainWindowViewModel = mainWindowViewModel;
      _dialogService = dialogService;
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
      _mainWindowViewModel.CurrentPage = new AddClientViewModel(_mainWindowViewModel, _dialogService);
      
   }

   [RelayCommand]
   private void NavigateToClientProfile(Client client)
   {
      ShowAddButton = false;
      _mainWindowViewModel.CurrentPage = new ClientProfileViewModel(client, _mainWindowViewModel, _dialogService);
   }
   
      

}
