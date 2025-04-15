using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using spike.Data;
using spike.Database;
using spike.Models;
using spike.Services;

namespace spike.ViewModels;

public partial class HomePageViewModel : PageViewModel
{  
    private readonly int _employeeColumnWidth = 300;
    
    [ObservableProperty]
    private double _canvasWidth;
    
    [ObservableProperty]
    private DateTimeOffset? _selectedDate;
    
    [ObservableProperty]
    private ObservableCollection<Times> _times;
    
    [ObservableProperty]
    private ObservableCollection<Employee> _employees;
    
    [ObservableProperty]
    private ObservableCollection<Appointment> _appointments;
    
    [ObservableProperty]
    private ObservableCollection<AppointmentViewModel> _appointmentViewModels;
    
    private readonly MainWindowViewModel _mainWindowViewModel;

    private DialogService _dialogService;
    
    public HomePageViewModel(MainWindowViewModel mainWindowViewModel, DialogService dialogService)
    {
        
        PageTitle = AppPageNames.Home;
        
        Times = new ObservableCollection<Times>();
        InitTimes();
        
        Employees = new ObservableCollection<Employee>();
        InitEmployees();
        
        // set date to today
        SelectedDate = DateTimeOffset.Now;
        
        CanvasWidth = _employeeColumnWidth * Employees.Count;

        _mainWindowViewModel = mainWindowViewModel;
        
        _dialogService = dialogService;
    
    }

    private void InitTimes()
    {
        
        Times.Add(new Times("5:00 am"));
        Times.Add(new Times("6:00 am"));
        Times.Add(new Times("7:00 am"));
        Times.Add(new Times("8:00 am"));
        Times.Add(new Times("9:00 am"));
        Times.Add(new Times("10:00 am"));
        Times.Add(new Times("11:00 am"));
        Times.Add(new Times("12:00 pm"));
        Times.Add(new Times("1:00 pm"));
        Times.Add(new Times("2:00 pm"));
        Times.Add(new Times("3:00 pm"));
        Times.Add(new Times("4:00 pm"));
        Times.Add(new Times("5:00 pm"));
        Times.Add(new Times("6:00 pm"));
        Times.Add(new Times("7:00 pm"));
        Times.Add(new Times("8:00 pm"));
        Times.Add(new Times("9:00 pm"));
        Times.Add(new Times("10:00 pm"));
        Times.Add(new Times("11:00 pm"));
        Times.Add(new Times("12:00 am"));
        Times.Add(new Times("1:00 am"));
        Times.Add(new Times("2:00 am"));
        Times.Add(new Times("3:00 am"));
        Times.Add(new Times("4:00 am"));
        
    }
    
    private void InitEmployees()
    {
        Employees = ReadFromDatabase.GetAllEmployees();
    }

    private void InitAppointments(DateTimeOffset? date)
    {
        Appointments = ReadFromDatabase.GetAppointments(date);
        AppointmentViewModels = CalculatePositions(Appointments);
    }
    
    partial void OnSelectedDateChanged(DateTimeOffset? value)
    {
        if (value != null)
        {
            InitAppointments(value);
        }
    }

    // calculates the position and width of each appointment block
    private ObservableCollection<AppointmentViewModel> CalculatePositions(ObservableCollection<Appointment> appointments)
    {
        // group all appointments by employee in hashmap
        var groupedAppointments = appointments.GroupBy(a => a.EmployeeStylists.EmployeeId);
        var result = new ObservableCollection<AppointmentViewModel>();
        
        
        foreach(var group in groupedAppointments)
        {
            //TODO get index of employee in employee list to make sure it is the correct column 
            int employeeColumn = GetEmployeeIndex(group.Key);
            // sort appointments by start time 
            var sortedAppointments = group.OrderBy(a => a.StartTime.Value.Hours);
            var rows = new List<List<Appointment>>();

            foreach (var appointment in sortedAppointments)
            {
                bool placed = false;
                foreach (var row in rows)
                {
                    // get all appointments that are within three hours of starting and add to the same row
                    if (row.Any(a => a.StartTime.Value.Hours + 3 >= appointment.StartTime.Value.Hours))
                    {
                        row.Add(appointment);
                        placed = true;
                        break;
                    }
                }

                if (!placed)
                {
                    rows.Add(new List<Appointment>{appointment});
                }
            }

            for (int i = 0; i < rows.Count; i++)
            {
                int appointmentIndex = 0;
                foreach (var appointment in rows[i])
                {
                    double column = (employeeColumn * _employeeColumnWidth) +
                                    (appointmentIndex * _employeeColumnWidth / rows[i].Count);
                    double width = (_employeeColumnWidth/rows[i].Count) - 4;
                    result.Add(new AppointmentViewModel(appointment, column, width));
                    appointmentIndex++;
                }
            }
        }
        
        return result;
    }

    private int GetEmployeeIndex(Int64 employeeId)
    {
        int index = 0;
        foreach (var employee in Employees)
        {
            if (employee.EmployeeId == employeeId)
                return index;
            index++;
        }

        return -1;
    }

    [RelayCommand]
    private void NavigateToAppointment(AppointmentViewModel appointment)
    {
        _mainWindowViewModel.CurrentPage = new FullAppointmentViewModel(appointment, _mainWindowViewModel, _dialogService);
    }
}

