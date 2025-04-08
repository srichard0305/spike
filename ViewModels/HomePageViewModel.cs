using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using spike.Data;
using spike.Database;
using spike.Models;

namespace spike.ViewModels;

public partial class HomePageViewModel : PageViewModel
{  
    private readonly int _employeeColumnWidth = 300;
    
    [ObservableProperty]
    private double _canvasWidth;
    
    private DateTimeOffset? _selectedDate;
    private DateTime _currentDate;
    
    [ObservableProperty]
    private ObservableCollection<Times> _times;
    
    [ObservableProperty]
    private ObservableCollection<Employee> _employees;
    [ObservableProperty]
    private ObservableCollection<AppointmentViewModel> _appointments;

    public HomePageViewModel()
    {
        PageTitle = AppPageNames.Home;
        
        Times = new ObservableCollection<Times>();
        InitTimes();
        
        Employees = new ObservableCollection<Employee>();
        InitEmployees();
        
        Appointments = new ObservableCollection<AppointmentViewModel>();
        InitAppointments();
        
        // set date to today
        SelectedDate = DateTimeOffset.Now;
        
        CanvasWidth = _employeeColumnWidth * Employees.Count;
        
        //Appointments = new ObservableCollection<AppointmentViewModel>(CalculatePositions(appointments));
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

    private void InitAppointments()
    {
        
    }

    // calculates the position and width of each appointment block
    private List<AppointmentViewModel> CalculatePositions(List<Appointment> appointments)
    {
        // group all appointments by employee
        var groupedAppointments = appointments.GroupBy(a => a.EmployeeStylists);
        var result = new List<AppointmentViewModel>();
        int employeeColumn = 0;
        
        foreach(var group in groupedAppointments)
        {
            // sort appointments by start time 
            var sortedAppointments = group.OrderBy(a => a.StartTime);
            var columns = new List<List<Appointment>>();

            foreach (var appointment in sortedAppointments)
            {
                bool placed = false;
                foreach (var column in columns)
                {
                    //TODO change string to timespan and datetime for processing 
                    
                    /*
                    // get all appointments that are within half and hour of starting and a add to the same column
                    if (column.Any(a => a.StartTime <= appointment.StartTime && a.StartTime.Minutes + 30 <= appointment.StartTime.Minutes))
                    {
                        column.Add(appointment);
                        placed = true;
                        break;
                    }
                    */
                }

                if (!placed)
                {
                    columns.Add(new List<Appointment>{appointment});
                }
            }

            for (int i = 0; i < columns.Count; i++)
            {
                int appointmentIndex = 0;
                foreach (var appointment in columns[i])
                {
                    double column = (employeeColumn * _employeeColumnWidth) +
                                    (appointmentIndex * _employeeColumnWidth / columns[i].Count);
                    double width = _employeeColumnWidth/columns[i].Count - 4;
                    result.Add(new AppointmentViewModel(appointment, column, width));
                    appointmentIndex++;
                }
            }

            employeeColumn++;
        }
        
        return result;
    }
    
    // gets selected date from datepicker
    public DateTimeOffset? SelectedDate
    {
        get => _selectedDate;
        set
        {
            if (_selectedDate != value)
            {
                _selectedDate = value;
                OnPropertyChanged(nameof(SelectedDate));
                
                _currentDate = _selectedDate?.Date ?? DateTime.Now;
            }
        }
    }
    
    // assigns selected date from datepicker to datetime for querying db
    public DateTime CurrentDate
    {
        get => _currentDate;
        private set
        {
            if (_currentDate != value)
            {
                _currentDate = value;
                OnPropertyChanged(nameof(CurrentDate));
                
                // todo 
                // load appointments for current date
            }
        }
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    
    
}

