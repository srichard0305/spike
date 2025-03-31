using CommunityToolkit.Mvvm.ComponentModel;
using spike.Models;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using spike.Data;

namespace spike.ViewModels;

public partial class ClientProfileViewModel : PageViewModel
{
    [ObservableProperty]
    private Client _client;

    private readonly MainWindowViewModel _mainWindowViewModel;

    public ClientProfileViewModel(Client client, MainWindowViewModel mainWindowViewModel)
    {
        PageTitle = AppPageNames.ClientProfile;
        Client = client;
        _mainWindowViewModel = mainWindowViewModel;

    }


    [RelayCommand]
    public void NavigateToEditProfile()
    {
        _mainWindowViewModel.CurrentPage = new EditClientProfileViewModel(Client);
        
    }

}