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

    private readonly MainWindowViewModel _mainWindowViewModel;
    
    private DialogService _dialogService;

    public ClientProfileViewModel(Client client, MainWindowViewModel mainWindowViewModel, DialogService dialogService)
    {
        PageTitle = AppPageNames.ClientProfile;
        Client = client;
        _mainWindowViewModel = mainWindowViewModel;
        _dialogService = dialogService;
    }
    
    [RelayCommand]
    private void NavigateToEditProfile()
    {
        _mainWindowViewModel.CurrentPage = new EditClientProfileViewModel(Client, _mainWindowViewModel, _dialogService);
        
    }

}