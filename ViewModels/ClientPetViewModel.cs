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

namespace spike.ViewModels;

public partial class ClientPetViewModel: PageViewModel
{
   private readonly PageFactory _pageFactory;
   
   [ObservableProperty]
   private PageViewModel? _currentPage;
   public ClientPetViewModel(PageFactory pageFactory) {
      _pageFactory = pageFactory;
      PageTitle = AppPageNames.ClientPet;
      NavigateToAddClient();
   }
   

   private ObservableCollection<Client> clients = new();
   
   
   [RelayCommand]
   private void NavigateToAddClient() => CurrentPage = _pageFactory.GetPageViewModel(AppPageNames.AddClient);


}
