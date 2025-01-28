using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace spike.ViewModels;

//partial allows ObservableProperty to change the contents of class 
public partial class MainWindowViewModel : ViewModelBase
{

    private const string buttonActiveClass = "active";
    
    //from toolkit, calls iNotifyPropertyChange adds public properties to /Dependencies/.Net9/Source Generators
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HomePageActive))]
    [NotifyPropertyChangedFor(nameof(ClientPetPageActive))]
    [NotifyPropertyChangedFor(nameof(EmployeePageActive))]
    [NotifyPropertyChangedFor(nameof(ReportsPageActive))]
    private ViewModelBase _currentPage;
    
    //create view models 
    private readonly HomePageViewModel _homePage = new HomePageViewModel();
    private readonly ClientPetViewModel _clientPetPage = new ClientPetViewModel();
    private readonly EmployeePageViewModel _employeePage = new EmployeePageViewModel();
    private readonly ReportsPageViewModel _reportsPage = new ReportsPageViewModel();
    
    // CurrentPage is generated with observable property
    public bool HomePageActive => CurrentPage == _homePage;
    public bool ClientPetPageActive => CurrentPage == _clientPetPage;
    public bool EmployeePageActive => CurrentPage == _employeePage;
    public bool ReportsPageActive => CurrentPage == _reportsPage;
    
    //-------------------------------------------------------------------------------------------------------//

    public MainWindowViewModel()
    {
        _currentPage = _homePage;
    }

    [RelayCommand]
    private void NavigateToHomePage()
    {
        CurrentPage = _homePage;
    }
    
    [RelayCommand]
    private void NavigateToClientPetPage()
    {
        CurrentPage = _clientPetPage;
    }
    
    [RelayCommand]
    private void NavigateToEmployeePage()
    {
        CurrentPage = _employeePage;
    }
    
    [RelayCommand]
    private void NavigateToReportsPage()
    {
        CurrentPage = _reportsPage;
    }

}