using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using spike.Data;
using spike.Factories;

namespace spike.ViewModels;

//partial allows ObservableProperty to change the contents of class 
public partial class MainWindowViewModel : ViewModelBase
{
    private readonly PageFactory _pageFactory;
    //from toolkit, calls iNotifyPropertyChange adds public properties to /Dependencies/.Net9/Source Generators
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HomePageActive))]
    [NotifyPropertyChangedFor(nameof(ClientPetPageActive))]
    [NotifyPropertyChangedFor(nameof(EmployeePageActive))]
    [NotifyPropertyChangedFor(nameof(ReportsPageActive))]
    private PageViewModel? _currentPage;
    
    // CurrentPage is generated with observable property
    // used for page nav    
    public bool HomePageActive => CurrentPage?.PageTitle == AppPageNames.Home;
    public bool ClientPetPageActive => CurrentPage?.PageTitle == AppPageNames.ClientPet;
    public bool EmployeePageActive => CurrentPage?.PageTitle == AppPageNames.Employees;
    public bool ReportsPageActive => CurrentPage?.PageTitle == AppPageNames.Reports;
    
    //-------------------------------------------------------------------------------------------------------//
    
    // remove default constructor after dev
    
    public MainWindowViewModel()
    {
        CurrentPage = new HomePageViewModel();
    }
    
    
    public MainWindowViewModel(PageFactory pageFactory)
    {
        _pageFactory = pageFactory;
        NavigateToHomePage();
    }

    [RelayCommand]
    private void NavigateToHomePage() => CurrentPage = _pageFactory.GetPageViewModel(AppPageNames.Home);
    
    [RelayCommand]
    private void NavigateToClientPetPage() => CurrentPage = _pageFactory.GetPageViewModel(AppPageNames.ClientPet);
   
    
    [RelayCommand]
    private void NavigateToEmployeePage() => CurrentPage = _pageFactory.GetPageViewModel(AppPageNames.Employees);
 
    
    [RelayCommand]
    private void NavigateToReportsPage() => CurrentPage = _pageFactory.GetPageViewModel(AppPageNames.Reports);


}