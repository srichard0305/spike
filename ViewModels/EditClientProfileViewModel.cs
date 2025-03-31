using CommunityToolkit.Mvvm.ComponentModel;
using spike.Data;
using spike.Models;

namespace spike.ViewModels;

public partial class EditClientProfileViewModel : PageViewModel
{
    [ObservableProperty]
    private Client _client;
    public EditClientProfileViewModel(Client client)
    {
        PageTitle = AppPageNames.EditClientProfile;
        Client = client;
        
    }
}