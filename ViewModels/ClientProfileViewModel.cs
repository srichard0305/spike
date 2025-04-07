using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using spike.Models;
using CommunityToolkit.Mvvm.Input;
using spike.Data;
using spike.Services;

namespace spike.ViewModels;

public partial class ClientProfileViewModel : PageViewModel
{
    [ObservableProperty]
    private Client _client;
    
    [ObservableProperty]
    private string? _birthday;

    private readonly MainWindowViewModel _mainWindowViewModel;
    
    private DialogService _dialogService;

    public ClientProfileViewModel(Client client, MainWindowViewModel mainWindowViewModel, DialogService dialogService)
    {
        PageTitle = AppPageNames.ClientProfile;
        
        Client = client;
        
        if(client.Pets != null && client.Pets.Any())
            FormatBirthday(client.Pets);
        
        _mainWindowViewModel = mainWindowViewModel;
        
        _dialogService = dialogService;
    }
    
    [RelayCommand]
    private void NavigateToEditProfile()
    {
        _mainWindowViewModel.CurrentPage = new EditClientProfileViewModel(Client, _mainWindowViewModel, _dialogService);
        
    }

    private void FormatBirthday(ObservableCollection<Pet> pets)
    {
        foreach (Pet pet in pets)
        {
            if (pet.Birthday != "")
            {
                var birthdayDateTimeOffset = DateTimeOffset.Parse(pet.Birthday);
                Birthday = birthdayDateTimeOffset.ToString("MMMM dd, yyyy", CultureInfo.InvariantCulture);
            }
            
        }
    }

}