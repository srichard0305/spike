using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using spike.Data;
using spike.Database;
using spike.Models;
using spike.Services;

namespace spike.ViewModels;

public partial class EmployeePageViewModel : PageViewModel
{
    [ObservableProperty]
    private ObservableCollection<Employee>? _employees;

    [ObservableProperty] 
    private bool _showAddButton;

    private readonly MainWindowViewModel _mainWindowViewModel;
   
    private DialogService _dialogService;
   
    public EmployeePageViewModel(MainWindowViewModel mainWindowViewModel, DialogService dialogService) {
        _mainWindowViewModel = mainWindowViewModel;
        _dialogService = dialogService;
        PageTitle = AppPageNames.Employees;
        Employees = new ObservableCollection<Employee>();
        ShowAddButton = true;
        InitClientsList();
    }
   
    private void InitClientsList()
    {
        Employees = ReadFromDatabase.GetAllEmployees();
    }
   

    [RelayCommand]
    private void NavigateToAddEmployee()
    {
        ShowAddButton = false;
        _mainWindowViewModel.CurrentPage = new AddEmployeeViewModel(_mainWindowViewModel, _dialogService);
      
    }

    [RelayCommand]
    private void NavigateToEmployeeProfile(Employee employee)
    {
        ShowAddButton = false;
        _mainWindowViewModel.CurrentPage = new EmployeeProfileViewModel(employee, _mainWindowViewModel, _dialogService);
    }
}