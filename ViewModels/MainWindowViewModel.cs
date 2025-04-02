using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using spike.Data;
using spike.Factories;
using spike.Interfaces;

namespace spike.ViewModels;

//partial allows ObservableProperty to change the contents of class 
public partial class MainWindowViewModel : ViewModelBase, IDialogProvider
{
    private readonly PageFactory _pageFactory;

    //from toolkit, calls iNotifyPropertyChange adds public properties to /Dependencies/.Net9/Source Generators
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HomePageActive))]
    [NotifyPropertyChangedFor(nameof(ClientPetPageActive))]
    [NotifyPropertyChangedFor(nameof(EmployeePageActive))]
    [NotifyPropertyChangedFor(nameof(ReportsPageActive))]
    private PageViewModel? _currentPage;
    
    [ObservableProperty]
    private DialogViewModel? _dialog;

    // CurrentPage is generated with observable property
    // used for page nav    
    public bool HomePageActive => CurrentPage?.PageTitle is AppPageNames.Home;
    public bool ClientPetPageActive => CurrentPage?.PageTitle is AppPageNames.ClientPet or AppPageNames.AddClient 
                                                                                        or AppPageNames.ClientProfile 
                                                                                        or AppPageNames.EditClientProfile;
    public bool EmployeePageActive => CurrentPage?.PageTitle is AppPageNames.Employees or AppPageNames.AddEmployee 
                                                                                       or AppPageNames.EmployeeProfile 
                                                                                       or AppPageNames.EditEmployeeProfile;
    public bool ReportsPageActive => CurrentPage?.PageTitle is AppPageNames.Reports;
    
    //-------------------------------------------------------------------------------------------------------//
    
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
    
    [RelayCommand]
    private void NavigateToBookAppointmentPage() => CurrentPage = _pageFactory.GetPageViewModel(AppPageNames.BookAppointment);
    
   
} 