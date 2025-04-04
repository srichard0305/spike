using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using spike.Data;
using spike.Models;
using spike.Services;

namespace spike.ViewModels;

public partial class EmployeeProfileViewModel : PageViewModel
{
    [ObservableProperty]
    private Employee _employee;
    
    private readonly MainWindowViewModel _mainWindowViewModel;
    
    private DialogService _dialogService;

    public EmployeeProfileViewModel(Employee employee, MainWindowViewModel mainWindowViewModel, DialogService dialogService)
    {
        PageTitle = AppPageNames.ClientProfile;
        
        Employee = employee;
        
        _mainWindowViewModel = mainWindowViewModel;
        
        _dialogService = dialogService;
    }
    
    [RelayCommand]
    private void NavigateToEditProfile()
    {
        _mainWindowViewModel.CurrentPage = new EditEmployeeProfileViewModel(Employee, _mainWindowViewModel, _dialogService);
        
    }
    
}